using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	public class ParticipantsService : Service, IParticipantsService
	{
		public ParticipantsService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public PageList<GroupedParticipantsModel> GetParticipants(int page, int quantity, ApplicationFilter filter)
		{
			if (page <= 0)
				page = 1;

			if (quantity <= 0 || quantity > 100)
				quantity = 10;

			var defaultProgramId = GetDefaultGrantProgramId();
			
			var participantForms = _dbContext.ParticipantForms
				.Include(pf => pf.GrantApplication)
				.AsNoTracking()
				.Where(pf => pf.GrantApplication.GrantOpening.GrantStream.GrantProgramId == defaultProgramId)
				.Where(pf => pf.GrantApplication.FileNumber != null)
				.Where(pf => pf.GrantApplication.ApplicationStateInternal != ApplicationStateInternal.Draft)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(filter.FileNumber))
				participantForms = participantForms.Where(pf => pf.GrantApplication.FileNumber.Contains(filter.FileNumber));

			if (!string.IsNullOrWhiteSpace(filter.Participant))
			{
				var searchTerms = filter.Participant.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
				participantForms = participantForms.Where(pf => searchTerms.Any(t => pf.FirstName.Contains(t))
				                                                || searchTerms.Any(t => pf.MiddleName.Contains(t))
																|| searchTerms.Any(t => pf.LastName.Contains(t)));
			}
			var total = participantForms.Count();

			var participants = new List<GroupedParticipantsModel>();

			foreach (var participantForm in participantForms)
			{
				var courseName = participantForm.GrantApplication.TrainingPrograms.FirstOrDefault()?.CourseTitle ?? "--";

				var paidToDate = participantForm.ParticipantCosts
					.Where(w => w.ClaimEligibleCost.Claim.ClaimState == ClaimState.ClaimApproved ||
					            w.ClaimEligibleCost.Claim.ClaimState == ClaimState.PaymentRequested ||
					            w.ClaimEligibleCost.Claim.ClaimState == ClaimState.ClaimPaid)
					.Sum(s => s.AssessedReimbursement);

				var participantsModel = new GroupedParticipantsModel
				{
					SIN = participantForm.SIN,
					ParticipantFormId = participantForm.Id,
					ParticipantFirstName = participantForm.FirstName,
					ParticipantMiddleName = participantForm.MiddleName,
					ParticipantLastName = participantForm.LastName,
					FileNumber = participantForm.GrantApplication.FileNumber,
					CourseName = courseName,
					EmployerName = participantForm.GrantApplication.Organization.LegalName,
					PaidToDate = paidToDate,
					LastApplicationDateTime = participantForm.DateAdded,
				};

				participants.Add(participantsModel);
			}

			var orderBy = filter.OrderBy != null && filter.OrderBy.Length > 0 ? filter.OrderBy : new[] { $"{nameof(GroupedParticipantsModel.SIN)} ASC" };

			participants = new List<GroupedParticipantsModel>(
				participants.AsQueryable()
					.OrderByProperty(orderBy)
					.Skip((page - 1) * quantity)
					.Take(quantity)
			);

			return new PageList<GroupedParticipantsModel>(page, quantity, total, participants);
		}
	}
}
