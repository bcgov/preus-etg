using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="Participant"/> class, provides the ORM a way to manage participants.  This provides a way to identify unique persons who have participated in training.
	/// </summary>
	public class Participant : EntityBase
	{
		// NOTE: The OldestAge and YoungestAge are modified by the static Participant constructor when OldestAge and YoungestAge are accessed.

		#region constants
		/// <summary>
		/// The oldest age a participant can be.
		/// </summary>
		public static readonly int OldestAge = 150;

		/// <summary>
		/// The youngest age a participant can be.
		/// </summary>
		public static readonly int YoungestAge = 16;
		#endregion

		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The participant's first name.
		/// </summary>
		[Required, MaxLength(100)]
		public string FirstName { get; set; }

		/// <summary>
		/// get/set - The participant's last name.
		/// </summary>
		[Required, MaxLength(100)]
		public string LastName { get; set; }

		/// <summary>
		/// get/set - The participant's birthdate.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Column(TypeName = "DATETIME2")]
		public DateTime BirthDate { get; set; }

		/// <summary>
		/// get/set - The participant's email address.
		/// </summary>
		[Required, MaxLength(500)]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		/// <summary>
		/// get - All of the participant forms linked to this participant.
		/// </summary>
		//public ICollection<ParticipantEnrollment> ParticipantEnrollments { get; set; } = new List<ParticipantEnrollment>();
		public ICollection<ParticipantForm> ParticipantForms { get; set; } = new List<ParticipantForm>();

		#endregion

		#region Constructors
		/// <summary>
		/// Intiailizes the configuration values.
		/// </summary>
		static Participant()
		{
			if (ConfigurationManager.AppSettings.HasKeys() && ConfigurationManager.AppSettings["ParticipantOldestAge"] != null)
			{
				int oldest; // NOSONAR
				if (int.TryParse(ConfigurationManager.AppSettings["ParticipantOldestAge"], out oldest))
				{
					Participant.OldestAge = oldest;
				}
			}

			if (ConfigurationManager.AppSettings.HasKeys() && ConfigurationManager.AppSettings["ParticipantYoungestAge"] != null)
			{
				int youngest; // NOSONAR
				if (int.TryParse(ConfigurationManager.AppSettings["ParticipantYoungestAge"], out youngest))
				{
					Participant.YoungestAge = youngest;
				}
			}
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Participant"/> object.
		/// </summary>
		public Participant()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Participant"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <param name="birthDate"></param>
		/// <param name="email"></param>
		public Participant(string firstName, string lastName, DateTime birthDate, string email)
		{
			if (String.IsNullOrEmpty(firstName))
				throw new ArgumentNullException(nameof(firstName));

			if (String.IsNullOrEmpty(lastName))
				throw new ArgumentNullException(nameof(lastName));

			var oldest = AppDateTime.UtcNow.AddYears(-1 * Participant.OldestAge).Date;
			var youngest = AppDateTime.UtcNow.AddYears(-1 * Participant.YoungestAge).Date;
			if (birthDate.Date < oldest || birthDate.Date > youngest)
				throw new ArgumentException($"The birthdate must be between '{oldest:yyyy}' and '{youngest:yyyy}'.", nameof(birthDate));

			if (String.IsNullOrEmpty(email))
				throw new ArgumentNullException(nameof(email));

			this.FirstName = firstName;
			this.LastName = lastName;
			this.BirthDate = birthDate;
			this.Email = email;
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="Participant"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="participantForm"></param>
		public Participant(ParticipantForm participantForm)
		{
			if (participantForm == null)
				throw new ArgumentNullException(nameof(participantForm));

			this.FirstName = participantForm.FirstName;
			this.LastName = participantForm.LastName;
			this.BirthDate = participantForm.BirthDate;
			this.Email = participantForm.EmailAddress;
		}
		#endregion

		#region Methods
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			// Cannot be older than the configured limit.
			// Cannot be younger than the configured limit.
			var oldest = AppDateTime.UtcNow.AddYears(-1 * Participant.OldestAge).Date;
			var youngest = AppDateTime.UtcNow.AddYears(-1 * Participant.YoungestAge).Date;
			if (this.BirthDate.Date < oldest || this.BirthDate.Date > youngest)
				yield return new ValidationResult($"The birthdate must be between '{oldest.ToLocalMidnight():yyyy}' and '{youngest.ToLocalMorning():yyyy}'.", new[] { nameof(this.BirthDate) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}