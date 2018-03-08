using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using EduadminCoursePortal.Attributes;
using EduadminCoursePortal.Resources;

namespace EduadminCoursePortal.Models.Booking
{
    public abstract class Question : IValidatableObject
    {
        [Required]
        public virtual int AnswerId { get; set; }
        public bool IsRequired { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public decimal? Price { get; set; }
        public int? AnswerNumber { get; set; }
        public bool ViewNrField { get; set; }
        public int QuestionTypeID { get; set; }

        private readonly IStringLocalizer<Resources.SharedResources> _localizer;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsRequired)
            {
                yield return new ValidationResult(String.Format(_localizer["MustSubmitText"], QuestionText)); //TODO: PhrasingTODO Åtgärda denna och samma på det andra stället
            }
        }
    }

    public class CheckQuestion : Question
    {
        [EnforceTrueIf("IsRequired", true, ErrorMessage = "MustCheckCheckBox")]
        public bool Checked { get; set; }
    }

    public class MultipleCheckQuestion : Question
    {
        public List<CheckQuestionAlternative> Alternatives { get; set; }
    }

    public class TextQuestion : Question
    {
        public bool DisplayAsTextArea { get; set; }
        [RequiredIf("IsRequired", true, ErrorMessage = "QuestionIsMandatory")]
        public string Answer { get; set; }
    }

    public class NumberQuestion : Question
    {
        [RequiredIf("IsRequired", true, ErrorMessage = "QuestionIsMandatory")]
        public int? Answer { get; set; }
    }

    public class InfoTextQuestion : Question
    {
    }

    public class SelectQuestion : Question
    {
        public SelectQuestion(SelectQuestionDisplayType display)
        {
            DisplayType = display;
        }

        [RequiredIf("IsRequired", true, ErrorMessage = "MustSelectAnOption")]
        public override int AnswerId
        {
            get => base.AnswerId;
            set => base.AnswerId = value;
        }
        public virtual List<QuestionAlternative> Alternatives { get; set; }
        public SelectQuestionDisplayType DisplayType { get; set; }
        public string DisplayLabel { get; set; }
    }

    public class CheckQuestionAlternative : QuestionAlternative
    {
        public bool Checked { get; set; }
    }

    public class QuestionAlternative
    {
        public string AlternativeText { get; set; }
        public int AnswerId { get; set; }
        public decimal? Price { get; set; }
        public bool Selected { get; set; }
    }

    public enum SelectQuestionDisplayType
    {
        Dropdown,
        Radiobutton
    }
}
