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
				.Include(pf => pf.GrantApplication.Organization)
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
			var orderByParts = filter.GetOrderByParts("SIN", true);

			// Have to sort here to avoid performance issues
			switch (orderByParts.PropertyName)
			{
				case "SIN":
					participantForms = participantForms.OrderByDynamic(pf => pf.SIN, orderByParts.IsAscending);
					break;

				case "ParticipantLastName":
					participantForms = participantForms.OrderByDynamic(pf => pf.LastName, orderByParts.IsAscending);
					break;

				case "ParticipantFirstName":
					participantForms = participantForms.OrderByDynamic(pf => pf.FirstName, orderByParts.IsAscending);
					break;

				case "FileNumber":
					participantForms = participantForms.OrderByDynamic(pf => pf.GrantApplication.FileNumber, orderByParts.IsAscending);
					break;

				case "CourseName":
					participantForms = participantForms.OrderByDynamic(pf => pf.GrantApplication.TrainingPrograms.FirstOrDefault().CourseTitle, orderByParts.IsAscending);
					break;

				case "EmployerName":
					participantForms = participantForms.OrderByDynamic(pf => pf.GrantApplication.Organization.LegalName, orderByParts.IsAscending);
					break;

				case "LastApplicationDateTime":
					participantForms = participantForms.OrderByDynamic(pf => pf.DateAdded, orderByParts.IsAscending);
					break;
			}

			participantForms = participantForms
					.Skip((page - 1) * quantity)
					.Take(quantity);

			var participants = new List<GroupedParticipantsModel>();
			foreach (var participantForm in participantForms)
			{
				var participantsModel = new GroupedParticipantsModel
				{
					SIN = participantForm.SIN,
					ParticipantFormId = participantForm.Id,
					ParticipantFirstName = participantForm.FirstName,
					ParticipantMiddleName = participantForm.MiddleName,
					ParticipantLastName = participantForm.LastName,
					FileNumber = participantForm.GrantApplication.FileNumber,
					CourseName = participantForm.GrantApplication.TrainingPrograms.FirstOrDefault()?.CourseTitle ?? "--",
					EmployerName = participantForm.GrantApplication.Organization.LegalName,
					LastApplicationDateTime = participantForm.DateAdded,
				};

				participants.Add(participantsModel);
			}

			//participants = new List<GroupedParticipantsModel>(participants);

			return new PageList<GroupedParticipantsModel>(page, quantity, total, participants);
		}
	}
}
