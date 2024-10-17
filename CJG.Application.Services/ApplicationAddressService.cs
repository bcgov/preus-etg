using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ApplicationAddressService"/> class, provides a way to manage <typeparamref name="ApplicationAddress"/> objects in the datastore.
	/// </summary>
	public class ApplicationAddressService : Service, IApplicationAddressService
	{
		private const int MaxCombinationAttempts = 500;

		public ApplicationAddressService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}

		/// <summary>
		/// Verify that the specified region name exist.
		/// If it doesn't create a new region for the specified country.
		/// </summary>
		/// <param name="regionName"></param>
		/// <param name="countryId"></param>
		/// <returns></returns>
		public Region VerifyOrCreateRegion(string regionName, string countryId)
		{
			if (string.IsNullOrWhiteSpace(regionName))
				throw new ArgumentException("Region is required", nameof(regionName));

			if (string.IsNullOrWhiteSpace(countryId))
				throw new ArgumentException("Country is required", nameof(countryId));

			if (countryId == Core.Entities.Constants.CanadaCountryId)
				throw new InvalidOperationException("Cannot add a new province to Canada.");

			regionName = regionName.Trim();
			var region = _dbContext.Regions.SingleOrDefault(x => x.CountryId == countryId && (x.Name == regionName || x.Id == regionName));

			if (region != null)
				return region;

			var allRegionIdsForCountry = _dbContext.Regions
				.Where(x => x.CountryId == countryId)
				.Select(x => x.Id.ToUpper())
				.ToList();

			if (!TryGenerateUniqueRegionId(allRegionIdsForCountry, regionName, out string newRegionId))
				throw new InvalidOperationException($"Can't find unique regionId for region name: {regionName}");

			region = new Region
			{
				CountryId = countryId,
				Name = regionName,
				Id = newRegionId
			};

			_dbContext.Regions.Add(region);

			return region;
		}

		/// <summary>
		/// Generate a unique Id for the region based on its name.
		/// </summary>
		/// <param name="existingRegionIds"></param>
		/// <param name="regionName"></param>
		/// <param name="newRegionId"></param>
		/// <returns></returns>
		internal static bool TryGenerateUniqueRegionId(IEnumerable<string> existingRegionIds, string regionName, out string newRegionId)
		{
			var regionIdsToCheck = existingRegionIds.ToList();
			newRegionId = null;

			if (string.IsNullOrWhiteSpace(regionName) || regionName.Length < 2)
				throw new InvalidOperationException($"Cannot create a region with this name '{regionName}'.");

			var index = 0; 
			foreach (var regionId in GetTwoLetterCombinations(regionName.ToUpperInvariant()))
			{
				newRegionId = regionId;

				if (!regionIdsToCheck.Contains(newRegionId))
					return true;

				if (index++ > MaxCombinationAttempts)
					return false;
			}

			return false;
		}

		/// <summary>
		/// Generate a list of two-letter values to represent the specified text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		internal static IEnumerable<string> GetTwoLetterCombinations(string text)
		{
			foreach (var combination in  text.ToCharArray().GetCombinations(2))
			{
				var letters = combination.ToList();
				yield return string.Concat(letters[0], letters[1]);
			}
		}

		/// <summary>
		/// If the specified address is orphaned, delete it.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="excludeApplicationId"></param>
		public void RemoveAddressIfNotUsed(ApplicationAddress address, int? excludeApplicationId = null)
		{
			var isSafeToRemove = excludeApplicationId.HasValue
				? !_dbContext.GrantApplications.Where(
						x =>
							x.Id != excludeApplicationId.Value &&
							(x.ApplicantMailingAddress.Id == address.Id ||
							 x.ApplicantPhysicalAddress.Id == address.Id))
					.Any()
				: !_dbContext.GrantApplications.Where(
						x =>
							x.ApplicantMailingAddress.Id == address.Id ||
							x.ApplicantPhysicalAddress.Id == address.Id)
					.Any();

			if (isSafeToRemove)
			{
				_dbContext.ApplicationAddresses.Remove(address);
			}
		}
	}
}
