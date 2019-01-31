using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduAdminApi;
using Microsoft.AspNetCore.Http;
using EduadminCoursePortal.Models.Booking;
using Session = EduadminCoursePortal.Models.Booking.Session;

namespace EduadminCoursePortal.BLL
{
    public interface IBookingService
    {
        Task<BookModel> GetNewBookModel(int eventId, int courseTemplateId, string backUrl);
        bool DateHasPassed(DateTime? date);
    }
    public class BookingService : IBookingService
    {
        private readonly IToken _token;

        public BookingService(IToken token)
        {
            _token = token;
        }

        public async Task<BookModel> GetNewBookModel(int eventId, int courseTemplateId, string backUrl)
        {
            var newToken = await _token.GetNewToken();
            var client = new EduAdminAPIClient.Client(newToken);

            var courseTemplateQuery = "?$select=CourseDescription,CourseTemplateId,RequireCivicRegistrationNumber&" + 
                                        "$expand=Events(" +
                                            "$select=EventId,EventName,ParticipantNumberLeft,StartDate,LastApplicationDate;" +
                                            "$expand=" +
                                                "PriceNames(" +
                                                    "$select=PriceNameId,PriceNameDescription,Price)," +
                                                "Sessions(" +
                                                    "$select=SessionId,StartDate,EndDate,SessionName,InternalSessionName;" +
                                                    "$expand=PriceNames($select=PriceNameId,Price)" +
                                                ");" +
                                      $"$filter=EventId eq {eventId})&$filter=ShowOnWeb eq true";

            var courseTemplate = await client.CourseTemplate.GetSingleAsync(courseTemplateId, courseTemplateQuery);
            var currentEvent = courseTemplate?.Events?.FirstOrDefault();

            var lastApplicationDate = currentEvent != null && currentEvent.LastApplicationDate.HasValue ? (DateTime?)currentEvent.LastApplicationDate.Value.DateTime : null;
            var startDate = currentEvent != null && currentEvent.StartDate.HasValue ? (DateTime?)currentEvent.StartDate.Value.DateTime : null;

            if (currentEvent == null || DateHasPassed(startDate) || DateHasPassed(lastApplicationDate))
            {
                return new BookModel
                {
                    HasError = true,
                    BackUrl = backUrl
                };
            }

            var questions = await GetBookingQuestions(eventId, client);

            return new BookModel
            {
                StartDate = startDate,
                LastApplicationDate = lastApplicationDate,
                Description = courseTemplate.CourseDescription,
                BookingQuestions = questions.BookingQuestions,
                EventId = currentEvent?.EventId ?? 0,
                EventName = currentEvent?.EventName,
                FullyBooked = currentEvent?.ParticipantNumberLeft != null && (currentEvent?.ParticipantNumberLeft ?? 0) <= 0, 
                CourseTemplateId = courseTemplate.CourseTemplateId ?? 0,
                Customer = CreateEmptyCustomer(),
                RequireCivicRegistrationNumber = courseTemplate.RequireCivicRegistrationNumber ?? false,
                Participants = GetAndSetParticipants(currentEvent, courseTemplate, questions.ParticipantQuestions),
                BackUrl = backUrl
            };
        }

        public bool DateHasPassed(DateTime? date)
        {
            return (date.HasValue && date.Value <= DateTime.Now);
        }

        private static List<Models.Participant> GetAndSetParticipants(CourseTemplateEvent currentEvent,
            CourseTemplate courseTemplate, List<Question> participantQuestions)
        {
            return new List<Models.Participant>
            {
                new Models.Participant
                {
                    RequireCivicRegistrationNumber = courseTemplate.RequireCivicRegistrationNumber ?? false,
                    PriceNames = GetParticipantPriceNames(currentEvent),
                    Sessions = GetParticipantSessions(currentEvent),
                    ParticipantQuestions = participantQuestions
                }
            };
        }

        private static List<PriceName> GetParticipantPriceNames(CourseTemplateEvent currentEvent)
        {
            return currentEvent?.PriceNames.Select(x => new PriceName
            {
                PriceNameId = x.PriceNameId ?? 0,
                Description = x.PriceNameDescription,
                Price = x.Price ?? 0
            }).ToList();
        }

        private static List<Session> GetParticipantSessions(CourseTemplateEvent currentEvent)
        {
            return currentEvent?.Sessions.Select(x => new Models.Booking.Session
            {
                EndDate = x.EndDate?.LocalDateTime ?? new DateTime(),
                Name = string.IsNullOrWhiteSpace(x.SessionName) ? x.SessionName : x.InternalSessionName,
                Participating = x.MandatoryParticipation ?? false,
                Price = x.PriceNames?.FirstOrDefault()?.Price ?? 0,
                PriceNameId = x.PriceNames?.FirstOrDefault()?.PriceNameId ?? 0,
                StartDate = x.StartDate?.LocalDateTime ?? new DateTime(),
                SessionId = x.SessionId ?? 0
            }).ToList();
        }

        private static Models.Booking.Customer CreateEmptyCustomer()
        {
            return new Models.Booking.Customer();
        }

        private async Task<EventQuestions> GetBookingQuestions(int eventId, EduAdminAPIClient.Client client)
        {
            var eventBookingQuestions = await client.Event.GetBookingQuestionsAsync(eventId, true);
            var bookQuestionValue = eventBookingQuestions.BookingQuestions;
            var participantQuestionValue = eventBookingQuestions.ParticipantQuestions;

            var bookingQuestions = new List<Question>(); 
            var participantQuestions = new List<Question>();

            foreach (var questionBase in bookQuestionValue)
            {
                if (questionBase.GetType() == typeof(EduAdminAPIClient.TextQuestion))
                    AddTextQuestion(bookingQuestions, (EduAdminAPIClient.TextQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.InfoTextQuestion))
                    AddInfotextQuestion(bookingQuestions, (EduAdminAPIClient.InfoTextQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.CheckboxQuestion))
                    AddCheckBoxQuestion(bookingQuestions, (EduAdminAPIClient.CheckboxQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.DropdownQuestion))
                    AddDropDownListQuestion(bookingQuestions, (EduAdminAPIClient.DropdownQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.NumberQuestion))
                    AddNumberQuestion(bookingQuestions, (EduAdminAPIClient.NumberQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.RadioQuestion))
                    AddRadioButtonQuestion(bookingQuestions, (EduAdminAPIClient.RadioQuestion)questionBase);
            }

            foreach (var questionBase in participantQuestionValue)
            {
                if (questionBase.GetType() == typeof(EduAdminAPIClient.TextQuestion))
                    AddTextQuestion(participantQuestions, (EduAdminAPIClient.TextQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.InfoTextQuestion))
                    AddInfotextQuestion(participantQuestions, (EduAdminAPIClient.InfoTextQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.CheckboxQuestion))
                    AddCheckBoxQuestion(participantQuestions, (EduAdminAPIClient.CheckboxQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.DropdownQuestion))
                    AddDropDownListQuestion(participantQuestions, (EduAdminAPIClient.DropdownQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.NumberQuestion))
                    AddNumberQuestion(participantQuestions, (EduAdminAPIClient.NumberQuestion)questionBase);

                if (questionBase.GetType() == typeof(EduAdminAPIClient.RadioQuestion))
                    AddRadioButtonQuestion(participantQuestions, (EduAdminAPIClient.RadioQuestion)questionBase);
            }

            return new EventQuestions
            {
                BookingQuestions = bookingQuestions,
                ParticipantQuestions = participantQuestions
            };
        }

        private static void AddDropDownListQuestion(List<Question> questions, EduAdminAPIClient.DropdownQuestion question)
        {
            var alternatives = question.Alternatives.OrderBy(y => y.SortKey);

            var displayLabel = alternatives.FirstOrDefault()?.AnswerText;
            var answerId = alternatives.FirstOrDefault()?.AnswerId ?? 0;

            questions.Add(new SelectQuestion(SelectQuestionDisplayType.Dropdown)
            {
                Alternatives = alternatives.Select(x => new QuestionAlternative
                {
                    AlternativeText = x.AnswerText,
                    AnswerId = x.AnswerId,
                    Price = x.Price,
                    Selected = x.AnswerId == answerId

                }).ToList(),
                QuestionTypeID = Constants.QuestionTypes.Dropdownlist,
                ViewNrField = question.HasNumberField,
                QuestionText = question.QuestionText,
                IsRequired = question.Mandatory,
                DisplayLabel = displayLabel,
                AnswerId = answerId
            });
        }

        private static void AddRadioButtonQuestion(List<Question> questions, EduAdminAPIClient.RadioQuestion question)
        {
            var alternatives = question.Alternatives.OrderBy(y => y.SortKey);

            questions.Add(new SelectQuestion(SelectQuestionDisplayType.Radiobutton)
            {
                Alternatives = alternatives.Select(x => new QuestionAlternative
                {
                    AlternativeText = x.AnswerText,
                    AnswerId = x.AnswerId,
                    Price = x.Price
                }).ToList(),
                QuestionText = question.QuestionText,
                IsRequired = question.Mandatory,
                DisplayLabel = null
            });
        }

        private static void AddNumberQuestion(List<Question> questions, EduAdminAPIClient.NumberQuestion question)
        {
            questions.Add(new NumberQuestion
            {
                AnswerId = question.AnswerId,
                QuestionTypeID = Constants.QuestionTypes.NumberQuestion,
                QuestionText = question.QuestionText,
                IsRequired = question.Mandatory,
                Price = question.Price
            });
        }

        private static void AddInfotextQuestion(List<Question> questions, EduAdminAPIClient.InfoTextQuestion question)
        {
            questions.Add(new InfoTextQuestion
            {
                QuestionText = question.QuestionText,
                QuestionTypeID = Constants.QuestionTypes.InfoText
            });
        }

        private static void AddTextQuestion(List<Question> questions, EduAdminAPIClient.TextQuestion question)
        {
            questions.Add(new TextQuestion
            {
                AnswerId = question.AnswerId,
                ViewNrField = question.HasNumberField,
                DisplayAsTextArea = question.IsTextArea,
                IsRequired = question.Mandatory,
                QuestionTypeID = question.IsTextArea ? Constants.QuestionTypes.TextArea : Constants.QuestionTypes.TextQuestion,
                QuestionText = question.QuestionText,
                Price = question.Price
            });
        }

        private static void AddCheckBoxQuestion(List<Question> questions, EduAdminAPIClient.CheckboxQuestion question)
        {
            if (question.Alternatives.Count == 1)
            {
                questions.Add(new CheckQuestion
                {
                    AnswerId = question.Alternatives.First().AnswerId,
                    QuestionTypeID = Constants.QuestionTypes.Checkbox,
                    IsRequired = question.Mandatory,
                    QuestionText = question.QuestionText,
                    AnswerText = question.Alternatives.First().AnswerText,
                    Price = question.Alternatives.First().Price
                });
            }
            else
            {
                questions.Add(new MultipleCheckQuestion
                {
                    Alternatives = question.Alternatives.Select(x => new CheckQuestionAlternative
                    {
                        AnswerId = x.AnswerId,
                        AlternativeText = x.AnswerText,
                        Price = x.Price
                    }).ToList(),
                    QuestionText = question.QuestionText,
                    IsRequired = question.Mandatory
                });
            }
        }
    }
}
