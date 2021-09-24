using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    /// <summary>
    /// <typeparamref name="IClaimTypeService"/> interface, provides a way to manage claim types.
    /// </summary>
    public interface IClaimTypeService: IService
    {
        /// <summary>
        /// Returns all the claim types for the system.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ClaimType> GetAll();

        /// <summary>
        /// Returns all the claim types for the system filtered by IsActive.
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        IEnumerable<ClaimType> GetAll(bool isActive);

        /// <summary>
        /// Returns the claim type for the specified Id.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NoContentException">The claim type does not exist.</exception>
        ClaimType GetClaimType(ClaimTypes type);
    }
}
