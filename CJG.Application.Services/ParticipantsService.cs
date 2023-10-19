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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ParticipantsService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		public PageList<GroupedParticipantsModel> GetParticipants(int page, int quantity, ApplicationFilter filter)
		{
			if (page <= 0)
				page = 1;

			if (quantity <= 0 || quantity > 100)
				quantity = 10;

			var participantForms = _dbContext.ParticipantForms
				.Include(pf => pf.GrantApplication)
				.AsNoTracking()
				.Where(pf => pf.GrantApplication.FileNumber != null)
				.Where(pf => pf.GrantApplication.ApplicationStateInternal != ApplicationStateInternal.Draft)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(filter.FileNumber))
				participantForms = participantForms.Where(pf => pf.GrantApplication.FileNumber.Contains(filter.FileNumber));

			if (!string.IsNullOrWhiteSpace(filter.Participant))
				participantForms = participantForms.Where(pf => pf.FirstName.Contains(filter.Participant)
				                                                || pf.MiddleName.Contains(filter.Participant)
				                                                || pf.LastName.Contains(filter.Participant));

			var total = participantForms.Count();

			var participants = new List<GroupedParticipantsModel>();

			foreach (var participantForm in participantForms)
			{
				var courseName = participantForm.GrantApplication.TrainingPrograms.FirstOrDefault()?.CourseTitle ?? "--";

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

		//TODO: This version of GetParticipants groups the results by SIN. Delete after non-grouped version confirmed as good. 
		//public PageList<GroupedParticipantsModel> GetParticipants(int page, int quantity, ApplicationFilter filter)
		//{
		//	if (page <= 0)
		//		page = 1;

		//	if (quantity <= 0 || quantity > 100)
		//		quantity = 10;

		//	var query = _dbContext.ParticipantForms
		//		.AsNoTracking()
		//		.Where(pf => pf.GrantApplication.FileNumber != null)
		//		.AsQueryable();

		//	if (!string.IsNullOrWhiteSpace(filter.FileNumber))
		//		query = query.Where(pf => pf.GrantApplication.FileNumber.Contains(filter.FileNumber));

		//	if (!string.IsNullOrWhiteSpace(filter.Participant))
		//		query = query.Where(pf => pf.FirstName.Contains(filter.Participant) || pf.LastName.Contains(filter.Participant));

		//	var total = query.Count();
		//	var grouped = query.GroupBy(pf => pf.SIN);

		//	var participants = new List<GroupedParticipantsModel>();

		//	foreach (var form in grouped)
		//	{
		//		var mostRecentForm = form.OrderByDescending(pf => pf.DateAdded).FirstOrDefault();

		//		if (mostRecentForm == null)
		//			continue;

		//		var forms = form
		//			.Where(x => x.LastName != mostRecentForm.LastName && x.FirstName != mostRecentForm.FirstName)
		//			.Select(x => x);

		//		var applicationNumbers = form
		//			.Where(pf => pf.GrantApplication.FileNumber != null)
		//			.Select(pf => pf.GrantApplication.FileNumber)
		//			.Distinct();

		//		var participantsModel = new GroupedParticipantsModel
		//		{
		//			SIN = form.Key,
		//			ParticipantFormId = mostRecentForm.Id,
		//			ParticipantFirstName = mostRecentForm.FirstName,
		//			ParticipantLastName = mostRecentForm.LastName,
		//			LastApplicationDateTime = mostRecentForm.DateAdded,
		//			ApplicationFileNumbers = applicationNumbers,
		//			ParticipantForms = forms.ToList()
		//		};

		//		participants.Add(participantsModel);
		//	}

		//	var orderBy = filter.OrderBy != null && filter.OrderBy.Length > 0 ? filter.OrderBy : new[] { $"{nameof(GroupedParticipantsModel.SIN)} ASC" };

		//	participants = new List<GroupedParticipantsModel>(
		//		participants.AsQueryable()
		//			.OrderByProperty(orderBy)
		//			.Skip((page - 1) * quantity)
		//			.Take(quantity)
		//	);

		//	return new PageList<GroupedParticipantsModel>(page, quantity, total, participants);
		//}
	}
}
