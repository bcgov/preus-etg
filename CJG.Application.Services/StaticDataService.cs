using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static CJG.Core.Entities.Constants;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="StaticDataServer"/> class, provides a way to fetch and cache various lookup collections.
	/// </summary>
	public class StaticDataService : Service, IStaticDataService
	{
		#region Properties
		private static readonly ConcurrentDictionary<string, IEnumerable<object>> _cache = new ConcurrentDictionary<string, IEnumerable<object>>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="StaticDataService"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public StaticDataService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Copy the properties of the source entity to the destination entity through reflection.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void CopyTo<TEntity>(TEntity source, TEntity destination) where TEntity : class
		{
			_dbContext.CopyTo<TEntity>(source, destination);
		}

		#region LookupTables
		/// <summary>
		/// Get the specified entities of type <typeparamref name="TEntity"/> and caches them.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="fetch"></param>
		/// <returns></returns>
		private IEnumerable<object> _GetAll<TEntity>(Func<IEnumerable<TEntity>> fetch) where TEntity : class
		{
			try
			{
				var type = typeof(TEntity);
				IEnumerable<object> results;
				if (!_cache.TryGetValue(type.Name, out results))
				{
					var items = fetch().ToList();
					_cache.TryAdd(type.Name, items);
					return items;
				}

				return results.Select(r => (TEntity)r).ToList();
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get '{0}' from datasource.", typeof(TEntity).Name);
				throw;
			}
		}

		/// <summary>
		/// Get the specified entities of type <typeparamref name="TEntity"/> and cache them.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <returns></returns>
		protected IEnumerable<TEntity> Get<TEntity, TKey>() where TEntity : LookupTable<TKey>
		{
			return _GetAll<TEntity>(() => _dbContext.Set<TEntity>().AsNoTracking().OrderBy(x => x.RowSequence).ThenBy(x => x.Caption)).Select(i => (TEntity)i);
		}

		/// <summary>
		/// Find the specified entity of type <typeparamref name="TEntity"/> with the specified key.
		/// Look in cache for the entity first.  If not found place collection in cache.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		protected TEntity Find<TEntity, TKey>(TKey key) where TEntity : LookupTable<TKey>
		{
			try
			{
				return Get<TEntity, TKey>().FirstOrDefault(l => l.Id.Equals(key));
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get '{0}' from datasource.", typeof(TEntity).Name);
				throw;
			}
		}

		/// <summary>
		/// Get all the delivery methods that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DeliveryMethod> GetDeliveryMethods()
		{
			return Get<DeliveryMethod, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified delivery method.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public DeliveryMethod GetDeliveryMethod(int id)
		{
			return _dbContext.DeliveryMethods.Find(id);
		}


		/// <summary>
		/// Get all the under represented groups that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<UnderRepresentedGroup> GetUnderRepresentedGroups()
		{
			return Get<UnderRepresentedGroup, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified under represented group.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public UnderRepresentedGroup GetUnderRepresentedGroup(int id)
		{
			return _dbContext.UnderRepresentedGroups.Find(id);
		}

		/// <summary>
		/// Get all the expected qualifications that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ExpectedQualification> GetExpectedQualifications()
		{
			return Get<ExpectedQualification, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified expected qualification.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ExpectedQualification GetExpectedQualification(int id)
		{
			return _dbContext.ExpectedQualifications.Find(id);
		}


		/// <summary>
		/// Get all the in-demand occupations that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InDemandOccupation> GetInDemandOccupations()
		{
			return Get<InDemandOccupation, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified in-demand occupation.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public InDemandOccupation GetInDemandOccupation(int id)
		{
			return _dbContext.InDemandOccupations.Find(id);
		}


		/// <summary>
		/// Get all the organization types that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<OrganizationType> GetOrganizationTypes()
		{
			return Get<OrganizationType, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified organization type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public OrganizationType GetOrganizationType(int id)
		{
			return _dbContext.OrganizationTypes.Find(id);
		}

		/// <summary>
		/// Get countries for Immigration dropdown, remove Canada and floats Bosnia, China and Poland to the top of list.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Country> GetImmigrationCountries()
		{
			var countryList = new List<string>() { BosniaCountryID, ChinaCountryID, PolandCountryID };

			return GetCountries().Where(x => x.Id != CanadaCountryId)
											.Where(c => c.IsActive)
											.OrderBy(x => countryList.All(c => c != x.Id))
											.ThenBy(x => x.RowSequence)
											.ThenBy(x => x.Name)
											.Select(c => c);
		}

		/// <summary>
		/// Get countries for training provider dropdown, removes Canada and floats United States to top of list.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Country> GetTrainingProviderCountries()
		{
			return GetCountries().Where(x => x.Id != CanadaCountryId)
											   .Where(c => c.IsActive && c.Id != CanadaCountryId)
											   .OrderBy(x => x.Id != USCountryID)
											   .ThenBy(x => x.RowSequence)
											   .ThenBy(x => x.Name)
											   .Select(c => c);
		}

		/// <summary>
		/// Get all the countries that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Country> GetCountries()
		{
			return _GetAll<Country>(() => _dbContext.Set<Country>().OrderBy(x => x.RowSequence).ThenBy(x => x.Name)).Select(r => (Country)r).Where(r => r.IsActive);
		}

		/// <summary>
		/// Get the specified province.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Country GetCountry(string id)
		{
			return _dbContext.Set<Country>().FirstOrDefault(c => c.Id == id);
		}

		/// <summary>
		/// Get all the regions that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Region> GetRegions()
		{
			return _GetAll<Region>(() => _dbContext.Set<Region>().Include(r => r.Country).OrderBy(x => x.RowSequence).ThenBy(x => x.Name)).Select(r => (Region)r).Where(r => r.IsActive);
		}

		/// <summary>
		/// Get the specified region in the specified country.
		/// </summary>
		/// <param name="countryId"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public Region GetRegion(string countryId, string id)
		{
			return _dbContext.Set<Region>().Include(r => r.Country).OrderBy(x => x.RowSequence).ThenBy(x => x.Name).FirstOrDefault(r => r.CountryId == countryId && r.Id == id);
		}

		/// <summary>
		/// Get all the provinces that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Region> GetProvinces()
		{
			return GetRegions().Where(r => r.CountryId == "CA");
		}

		/// <summary>
		/// Get the specified province.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Region GetProvince(string id)
		{
			return GetRegion("CA", id);
		}


		/// <summary>
		/// Get all the skills focuses that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SkillsFocus> GetSkillsFocuses()
		{
			return Get<SkillsFocus, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified skills focus.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public SkillsFocus GetSkillsFocus(int id)
		{
			return _dbContext.SkillsFocuses.Find(id);
		}


		/// <summary>
		/// Get all the skill levels that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SkillLevel> GetSkillLevels()
		{
			return Get<SkillLevel, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified skill level.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public SkillLevel GetSkillLevel(int id)
		{
			return _dbContext.SkillLevels.Find(id);
		}


		/// <summary>
		/// Get all the training levels that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TrainingLevel> GetTrainingLevels()
		{
			return Get<TrainingLevel, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified training level.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TrainingLevel GetTrainingLevel(int id)
		{
			return _dbContext.TrainingLevels.Find(id);
		}


		/// <summary>
		/// Get all the legal structures that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<LegalStructure> GetLegalStructures()
		{
			return Get<LegalStructure, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified legal structure.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public LegalStructure GetLegalStructure(int id)
		{
			return _dbContext.LegalStructures.Find(id);
		}

		/// <summary>
		/// Get all the training provider types that are ctive.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TrainingProviderType> GetTrainingProviderTypes()
		{
			return Get<TrainingProviderType, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified training provider type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TrainingProviderType GetTrainingProviderType(int id)
		{
			return _dbContext.TrainingProviderTypes.Find(id);
		}

		/// <summary>
		/// Get all the priority sectors that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PrioritySector> GetPrioritySectors()
		{
			return Get<PrioritySector, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified priority sector.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public PrioritySector GetPrioritySector(int id)
		{
			return _dbContext.PrioritySectors.Find(id);
		}

		/// <summary>
		/// Get all the expense types that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ExpenseType> GetExpenseTypes()
		{
			return Get<ExpenseType, ExpenseTypes>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified expense type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ExpenseType GetExpenseType(ExpenseTypes type)
		{
			return GetExpenseTypes().FirstOrDefault(ct => ct.Id == type);
		}

		/// <summary>
		/// Get all the Canadian statuses that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CanadianStatus> GetCanadianStatuses()
		{
			return Get<CanadianStatus, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified Canadian status.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CanadianStatus GetCanadianStatus(int id)
		{
			return _dbContext.CanadianStatuses.Find(id);
		}

		/// <summary>
		/// Get all the aboriginal bands that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<AboriginalBand> GetAboriginalBands()
		{
			return Get<AboriginalBand, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified aboriginal band.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public AboriginalBand GetAboriginalBand(int id)
		{
			return _dbContext.AboriginalBands.Find(id);
		}

		/// <summary>
		/// Get all the application types that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ApplicationType> GetApplicationTypes()
		{
			return Get<ApplicationType, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified application type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ApplicationType GetApplicationType(int id)
		{
			return _dbContext.ApplicationTypes.Find(id);
		}

		/// <summary>
		/// Get all the marital status that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<MaritalStatus> GetMaritalStatuses()
		{
			return Get<MaritalStatus, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified marital status.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public MaritalStatus GetMaritalStatus(int id)
		{
			return _dbContext.MaritalStatuses.Find(id);
		}

		/// <summary>
		/// Get all the federal official languages that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<FederalOfficialLanguage> GetFederalOfficialLanguages()
		{
			return Get<FederalOfficialLanguage, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified federal official language.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public FederalOfficialLanguage GetFederalOfficialLanguage(int id)
		{
			return _dbContext.FederalOfficialLanguages.Find(id);
		}


		/// <summary>
		/// Get all the education levels that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<EducationLevel> GetEducationLevels()
		{
			return Get<EducationLevel, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified education level.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public EducationLevel GetEducationLevel(int id)
		{
			return _dbContext.EducationLevels.Find(id);
		}


		/// <summary>
		/// Get all the employment types that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<EmploymentType> GetEmploymentTypes()
		{
			return Get<EmploymentType, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified employment type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public EmploymentType GetEmploymentType(int id)
		{
			return _dbContext.EmploymentTypes.Find(id);
		}


		/// <summary>
		/// Get all the employment statuses that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<EmploymentStatus> GetEmploymentStatuses()
		{
			return Get<EmploymentStatus, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified employment status.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public EmploymentStatus GetEmploymentStatus(int id)
		{
			return _dbContext.EmploymentStatuses.Find(id);
		}

		/// <summary>
		/// Get all the training results that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TrainingResult> GetTrainingResults()
		{
			return Get<TrainingResult, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified training result.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TrainingResult GetTrainingResult(int id)
		{
			return _dbContext.TrainingResults.Find(id);
		}

		/// <summary>
		/// Get all the note types that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NoteType> GetNoteTypes()
		{
			return Get<NoteType, NoteTypes>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified note type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public NoteType GetNoteType(NoteTypes type)
		{
			return _dbContext.NoteTypes.Find(type);
		}

		/// <summary>
		/// Get all the Participant EmploymentStatuses that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ParticipantEmploymentStatus> GetParticipantEmploymentStatuses()
		{
			return Get<ParticipantEmploymentStatus, int>().Where(e => e.IsActive).OrderBy(e => e.RowSequence).ThenBy(e => e.Caption);
		}

		/// <summary>
		/// Get all the Under RepresentedPopulations that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<UnderRepresentedPopulation> GetUnderRepresentedPopulations()
		{
			return Get<UnderRepresentedPopulation, int>().Where(e => e.IsActive);
		}
		/// <summary>
		/// Get all the Applicant Organization Types that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ApplicantOrganizationType> GetApplicantOrganizationTypes()
		{
			return Get<ApplicantOrganizationType, int>().Where(e => e.IsActive);
		}
		/// <summary>
		/// Get all the Vulnerable Groups that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<VulnerableGroup> GetVulnerableGroups()
		{
			return Get<VulnerableGroup, int>().Where(e => e.IsActive);
		}


		#endregion

		/// <summary>
		/// Get the <typeparamref name="DbSet"/> of type <typeparamref name="TEntity"/>.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <returns></returns>
		protected DbSet<TEntity> Get<TEntity>() where TEntity : class
		{
			return _dbContext.Set<TEntity>();
		}

		/// <summary>
		/// Get all the fiscal years that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<FiscalYear> GetFiscalYears()
		{
			return Get<FiscalYear>().OrderBy(fy => fy.StartDate).ThenBy(fy => fy.Caption);
		}

		/// <summary>
		/// Get the specified fiscal year.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public FiscalYear GetFiscalYear(int id)
		{
			if (id == 0)
				return _dbContext.FiscalYears
					.AsNoTracking()
					.FirstOrDefault(x => x.StartDate <= AppDateTime.UtcNow && x.EndDate >= AppDateTime.UtcNow);

			return _dbContext.FiscalYears.Find(id);
		}

		/// <summary>
		/// Get all the training periods for a stream.
		/// </summary>
		/// <returns></returns>
		[Obsolete]
		public IEnumerable<TrainingPeriod> GetTrainingPeriods(int grantStreamId)
		{
			return Get<TrainingPeriod>()
				.Where(x => x.GrantStreamId == grantStreamId)
				.OrderBy(fy => fy.StartDate)
				.ThenBy(fy => fy.Caption);
		}

		/// <summary>
		/// Get the all the training periods within the specified fiscal year.
		/// </summary>
		/// <param name="fiscalYearId"></param>
		/// <returns></returns>
		[Obsolete("Examine if this is valid anymore when not filtered down by stream, or with the IsActive filtering.")]
		public IEnumerable<TrainingPeriod> GetTrainingPeriodsForFiscalYear(int fiscalYearId)
		{
			return Get<TrainingPeriod>()
				.Where(tp => tp.IsActive)
				.Where(tp => tp.FiscalYearId == fiscalYearId)
				.OrderBy(fy => fy.StartDate)
				.ThenBy(fy => fy.Caption);
		}

		[Obsolete("Examine if this is valid anymore when not filtered down by stream, or with the IsActive filtering.")]
		public IEnumerable<TrainingPeriod> GetTrainingPeriodsForFiscalYear(int fiscalYearId, int grantProgramId)
		{
			return Get<TrainingPeriod>()
				.Where(tp => tp.IsActive)
				.Where(tp => tp.FiscalYearId == fiscalYearId)
				.Where(tp => tp.GrantStream.GrantProgramId == grantProgramId)
				.OrderBy(fy => fy.StartDate)
				.ThenBy(fy => fy.Caption);
		}

		public IEnumerable<TrainingPeriod> GetTrainingPeriodsForFiscalYear(int fiscalYearId, int grantProgramId, int grantStreamId)
		{
			return Get<TrainingPeriod>()
				.Where(tp => tp.IsActive)
				.Where(tp => tp.FiscalYearId == fiscalYearId)
				.Where(tp => tp.GrantStream.GrantProgramId == grantProgramId)
				.Where(tp => tp.GrantStream.Id == grantStreamId)
				.OrderBy(fy => fy.StartDate)
				.ThenBy(fy => fy.Caption);
		}

		/// <summary>
		/// Get the specified training period.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TrainingPeriod GetTrainingPeriod(int id)
		{
			return _dbContext.TrainingPeriods.Find(id);
		}

		/// <summary>
		/// Get all the rate formats.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RateFormat> GetRateFormats()
		{
			return _GetAll(() => _dbContext.RateFormats.AsNoTracking().Where(r => r.Rate >= 0)).Select(r => (RateFormat)r);
		}

		/// <summary>
		/// Get the specified rate format.
		/// </summary>
		/// <param name="rate"></param>
		/// <returns></returns>
		public RateFormat GetRateFormat(double rate)
		{
			return GetRateFormats().FirstOrDefault(r => Math.Abs(r.Rate - rate) < TypeExtensions.FloatTolerance);
		}

		/// <summary>
		/// Get all the active claim types.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ClaimType> GetClaimTypes()
		{
			return _GetAll(() => _dbContext.ClaimTypes.AsNoTracking().Where(ct => ct.IsActive).OrderBy(ct => ct.RowSequence).ThenBy(ct => ct.Caption)).Select(ct => (ClaimType)ct);
		}

		public IEnumerable<ProgramType> GetProgramTypes()
		{
			return _GetAll(() => _dbContext.ProgramTypes.AsNoTracking().Where(ct => ct.IsActive).OrderBy(ct => ct.RowSequence).ThenBy(ct => ct.Caption)).Select(ct => (ProgramType)ct);
		}

		/// <summary>
		/// Get the claim type for the specified id.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ClaimType GetClaimType(ClaimTypes type)
		{
			return GetClaimTypes().FirstOrDefault(ct => ct.Id == type);
		}

		public IEnumerable<ServiceType> GetServiceTypes()
		{
			return _GetAll(() => _dbContext.ServiceTypes.AsNoTracking().Where(t => t.IsActive).OrderBy(ct => ct.RowSequence).ThenBy(ct => ct.Caption)).Select(s => (ServiceType)s);
		}

		/// <summary>
		/// Get all the risk classifications that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RiskClassification> GetRiskClassifications()
		{
			return Get<RiskClassification, int>().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified risk classification.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public RiskClassification GetRiskClassification(int id)
		{
			return _dbContext.RiskClassifications.Find(id);
		}

		/// <summary>
		/// Get all the notification types that are active.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NotificationType> GetNotificationTypes()
		{
			return Get<NotificationType>().AsNoTracking().Where(e => e.IsActive);
		}

		/// <summary>
		/// Get the specified notification types.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public NotificationType GetNotificationType(int id)
		{
			return _dbContext.NotificationTypes.Find(id);
		}
		#endregion
	}
}
