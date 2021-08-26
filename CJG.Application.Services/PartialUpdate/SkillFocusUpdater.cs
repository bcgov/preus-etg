using System;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Application.Services.PartialUpdate
{
    internal class SkillFocusFields
    {
        public SkillsFocus SkillFocus { get; set; }
        public InDemandOccupation InDemandOccupation { get; set; }
        public TrainingLevel TrainingLevel { get; set; }
    }

    internal class SkillFocusUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : GrantApplication
    {
        private readonly Action<TEntity, SkillFocusFields> _setValue;
        private readonly Func<int, SkillsFocus> _getSkillFocus;
        private readonly Func<int, InDemandOccupation> _getInDemandOccupation;
        private readonly Func<int, TrainingLevel> _getTrainingLevel;

        public SkillFocusUpdater(string systemNoteText, Action<TEntity, SkillFocusFields> setValue,
            Func<int, SkillsFocus> getSkillFocus,
            Func<int, InDemandOccupation> getInDemandOccupation,
            Func<int, TrainingLevel> getTrainingLevel, Action<TEntity> ensureExists = null, bool allowEmptyValue = true) 
            : base(systemNoteText, null, ensureExists, allowEmptyValue)
        {
            _setValue = setValue;
            _getSkillFocus = getSkillFocus;
            _getInDemandOccupation = getInDemandOccupation;
            _getTrainingLevel = getTrainingLevel;

            ConvertEntityToText = ConvertApplicationToText;
        }

        public override void UpdateValue(TEntity entity, string[] arr)
        {
            Validate(arr);

            var skillFocusFields = ConvertArrayToSkillFocusFields(arr);

            _setValue(entity, skillFocusFields);
        }

        private void Validate(string[] arr)
        {
            var skillFocusFields = ConvertArrayToSkillFocusFields(arr);

            if(skillFocusFields == null)
                throw new ApplicationException("Can't convert selected values to SkillFocusFields");

            if(skillFocusFields.SkillFocus == null)
                throw new ApplicationException("Skill Focus is required");

        }

        private SkillFocusFields ConvertArrayToSkillFocusFields(string[] arr)
        {
            if(arr == null || !arr.Any()) return null;

            var skillFocusFields = new SkillFocusFields();

            int skillFocus;
            if (arr.Length > 0 && int.TryParse(arr[0], out skillFocus))
            {
                skillFocusFields.SkillFocus = _getSkillFocus(skillFocus);

                int inDemandOccupation;
                if (arr.Length > 1 && int.TryParse(arr[1], out inDemandOccupation))
                {
                    skillFocusFields.InDemandOccupation = _getInDemandOccupation(inDemandOccupation);

                    int trainingLevel;
                    if (arr.Length > 2 && int.TryParse(arr[2], out trainingLevel))
                    {
                        skillFocusFields.TrainingLevel = _getTrainingLevel(trainingLevel);
                    }
                }
            }

            return skillFocusFields;
        }

        public override string GetNewValueText(string[] arr)
        {
            return ConvertArrayToFormattedText(arr);
        }

        private string ConvertArrayToFormattedText(string[] arr)
        {
            return ConvertSkillFocusFieldsToText(ConvertArrayToSkillFocusFields(arr));
        }


        public string ConvertApplicationToText(GrantApplication app)
        {
            return ConvertSkillFocusFieldsToText(ConvertApplicationToSkillFocusFields(app));
        }

        private string ConvertSkillFocusFieldsToText(SkillFocusFields skillFocusFields)
        {
            if (skillFocusFields?.SkillFocus == null) return "None";

            SystemNoteText = PartialEntityUpdateConstants.TrainingProgramSkillFocus;

            var result = skillFocusFields.SkillFocus.Caption;

            if (skillFocusFields.InDemandOccupation != null)
            {
                result += "\\" + skillFocusFields.InDemandOccupation.Caption;
            }

            if (skillFocusFields.TrainingLevel != null)
            {
                result += "\\" + skillFocusFields.TrainingLevel.Caption;
            }

            return result;
        }

        private SkillFocusFields ConvertApplicationToSkillFocusFields(GrantApplication app)
        {
            return new SkillFocusFields()
            {
                SkillFocus = app.TrainingPrograms.FirstOrDefault().SkillFocus,
                InDemandOccupation = app.TrainingPrograms.FirstOrDefault().InDemandOccupation,
                TrainingLevel = app.TrainingPrograms.FirstOrDefault().TrainingLevel
            };
        }


    }
}