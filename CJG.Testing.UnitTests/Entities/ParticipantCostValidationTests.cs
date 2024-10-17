using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class ParticipantCostValidationTests
    {
        ServiceHelper helper { get; set; }        

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();   

            helper = new ServiceHelper(typeof(ParticipantService), user);
            helper.MockContext();
            helper.MockDbSet<ParticipantCost>();
            helper.MockDbSet<ClaimEligibleCost>();
            helper.MockDbSet<EligibleCost>();
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Cost_Not_Associated_With_Claim_Eligible_Cost()
        {
            var participantCost = new ParticipantCost();
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant cost must be associated with a claim eligible cost.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Cost_Not_Associated_With_Participant_Form()
        {
            var participantCost = new ParticipantCost();
            var service = helper.Create<ParticipantService>();

            // Act 
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant cost must be associated with a participant form.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Cost_Not_LessThan_Or_Equal_Claimed_Maximum_Participant_Cost()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                ClaimParticipantCost = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant cost must be less than or equal to the claimed maximum participant cost.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Cost_Not_LessThan_Or_Equal_Agreed_Maximum_Participant_Cost()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                ClaimParticipantCost = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant cost must be less than or equal to the agreed maximum participant cost.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Cost_Not_Equal_Reimbursement_Amount_And_Employer_Contribution()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                ClaimParticipantCost = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant cost must equal the reimbursement amount and employer contribution.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Actual_Cost_Not_Exceeds_Maximum_Cost_Per_Participant()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                ClaimReimbursement = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("Actual Cost for Participant exceeds Maximum Cost per Participant.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Reimbursement_Amount_Not_Less_Or_Equal_Agreed_Maximum_Participant_Reimbursement()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                ClaimReimbursement = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant reimbursement must be less than or equal to the agreed maximum participant reimbursement.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Assessed_Participant_Cost_Not_LessThan_Or_Equal_Assessed_Maximum_Participant_Cost()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                AssessedParticipantCost = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The assessed participant cost must be less than or equal to the assessed maximum participant cost.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Assessed_Participant_Cost_Not_LessThan_Or_Equal_Agreed_Maximum_Participant_Cost()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                AssessedParticipantCost = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant cost must be less than or equal to the agreed maximum participant cost.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Assessed_Participant_Cost_Not_Equal_Assessed_Reimbursement_Amount_And_Assessed_Employer_Contribution()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 0,
                AssessedParticipantCost = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 0,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToList();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The assessed participant cost must equal the assessed reimbursement amount and assessed employer contribution.")));
        }

        [Ignore, TestMethod, TestCategory("Participant Cost"), TestCategory("Validate")]
        public void Validate_When_Participant_Reimbursement_Not_Equal_Agreed_Maximum_Participant_Reimbursement()
        {
            var participantCost = new ParticipantCost()
            {
                ParticipantFormId = 1,
                AssessedReimbursement = 1,
                ClaimEligibleCost = new ClaimEligibleCost()
                {
                    Claim = new Claim()
                    {
                        Id = 1,
                        GrantApplication = new GrantApplication() { Id = 1 }
                    }
                }
            };
            var service = helper.Create<ParticipantService>();

            // Act
            var validationResults = service.Validate(participantCost).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage.Contains("The participant reimbursement must be less than or equal to the agreed maximum participant reimbursement.")));
        }
    }
}