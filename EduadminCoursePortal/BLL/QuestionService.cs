using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduAdminApi;
using EduadminCoursePortal.Models.Booking;

namespace EduadminCoursePortal.BLL
{
    public interface IQuestionService
    {
        List<AddBookingAnswer> GetBookingAnswers(List<QuestionAnswer> answers);
    }
    public class QuestionService : IQuestionService
    {
        public List<AddBookingAnswer> GetBookingAnswers(List<QuestionAnswer> answers)
        {
            var bookingAnswers = new List<AddBookingAnswer>();

            if (answers == null)
                return bookingAnswers;

            foreach (var questionAnswer in answers)
            {
                var answerNumber = GetAnswerNumber(questionAnswer.AnswerNumber);

                if (questionAnswer.Alternatives?.Any() ?? false)
                {
                    bookingAnswers.AddRange(from questionAnswerAlternative in questionAnswer.Alternatives
                        where questionAnswerAlternative.Checked
                        select new AddBookingAnswer
                        {
                            AnswerId = questionAnswerAlternative.AnswerId,
                            AnswerNumber = answerNumber,
                            AnswerValue = questionAnswerAlternative.Checked
                        });
                }
                else
                {
                    var answerValue = GetQuestionAnswerValue(questionAnswer);

                    if (answerValue is bool b)
                    {
                        if (b)
                            bookingAnswers.Add(new AddBookingAnswer
                            {
                                AnswerId = questionAnswer.AnswerId,
                                AnswerNumber = answerNumber,
                                AnswerValue = answerValue
                            });
                    }
                    else if (answerValue != null)
                    {
                        bookingAnswers.Add(new AddBookingAnswer
                        {
                            AnswerId = questionAnswer.AnswerId,
                            AnswerNumber = answerNumber,
                            AnswerValue = answerValue
                        });
                    }
                }
            }

            return bookingAnswers;
        }

        public int? GetAnswerNumber(string answerNumber)
        {
            if (int.TryParse(answerNumber, out var number))
                return number;

            return null;
        }

        public object GetQuestionAnswerValue(QuestionAnswer answerValue)
        {
            var questionTypeID = answerValue.QuestionTypeID;

            if (IsTextQuestion(questionTypeID) || IsTextArea(questionTypeID) || IsNumberQuestion(questionTypeID))
                return answerValue.Answer;

            return !IsCheckBoxQuestion(questionTypeID) || answerValue.Checked;
        }

        public bool IsTextQuestion(int questionTypeID)
        {
            return questionTypeID == Constants.QuestionTypes.TextQuestion;
        }

        public bool IsTextArea(int questionTypeID)
        {
            return questionTypeID == Constants.QuestionTypes.TextArea;
        }

        public bool IsNumberQuestion(int questionTypeID)
        {
            return questionTypeID == Constants.QuestionTypes.NumberQuestion;
        }

        public bool IsCheckBoxQuestion(int questionTypeID)
        {
            return questionTypeID == Constants.QuestionTypes.Checkbox;
        }
    }
}
