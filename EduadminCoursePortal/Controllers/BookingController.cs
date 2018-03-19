using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EduAdminApi;
using Microsoft.Extensions.Localization;
using EduadminCoursePortal.BLL;
using EduadminCoursePortal.Resources;
using EduadminCoursePortal.Models.Booking;

namespace EduadminCoursePortal.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IQuestionService _questionService;
        private readonly IToken _token;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public BookingController(IBookingService bookingService, IQuestionService questionService, IToken token, IStringLocalizer<SharedResources> localizer)
        {
            _bookingService = bookingService;
            _questionService = questionService;
            _token = token;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<ActionResult> Index(int eventId, int courseTemplateId)
        {
            var model = await _bookingService.GetNewBookModel(eventId, courseTemplateId, HttpContext.Request.Headers["Referer"].ToString());

            return View("Index", model);
        }

        private JsonResult GetJsonResultFromModelStateErrors()
        {
            return Json(new CreateBookingResult
            {
                Errors = ModelState.Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(x => x.Key, x => x.Value.Errors.Select(err => err.ErrorMessage).ToList())
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateBooking(CreateBookingModel createBookingModel)
        {
            createBookingModel = CreateBookingModelWithoutInfoQuestions(createBookingModel);

            if (createBookingModel.EventId <= 0)
                throw new Exception(_localizer["AnErrorOccurred"]);

            if (!ModelState.IsValid)
                return GetJsonResultFromModelStateErrors();

            var newToken = await _token.GetNewToken();
            var client = new EduAdminAPIClient.Client(newToken);
            
            var booking = new PostBooking
            {
                Answers = _questionService.GetBookingAnswers(createBookingModel.BookingQuestions),
                ContactPerson = new PostBookingContactPerson
                {
                    FirstName = createBookingModel.CustomerContact?.FirstName,
                    LastName = createBookingModel.CustomerContact?.LastName,
                    Email = createBookingModel.CustomerContact?.Email
                },
                Customer = new PostBookingCustomer
                {
                    CustomerName = createBookingModel.Customer?.CustomerName,
                    OrganisationNumber = createBookingModel.Customer?.InvoiceOrgnr,
                    Address = createBookingModel.Customer?.Address1,
                    Zip = createBookingModel.Customer?.Zip,
                    City = createBookingModel.Customer?.City
                },
                EventId = createBookingModel.EventId,
                Participants = createBookingModel.Participants?.Select(x => new BookingParticipant
                {
                    Answers = _questionService.GetBookingAnswers(x.ParticipantQuestions),
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    CivicRegistrationNumber = x.CivicRegistrationNumber,
                    PriceNameId = x.PriceNameId != 0 ? (int?)x.PriceNameId : null,
                    Sessions = x.Sessions?.Where(s => s.Participating).Select(y => new BookingParticipantSession
                    {
                        SessionId = y.SessionId,
                        PriceNameId = y.PriceNameId
                    }).ToList()
                }).ToList(),
                SendConfirmationEmail = new SendInfoBase
                {
                    SendToParticipants = true
                },
            };

            BookingCreated newBooking;

            try
            {
                newBooking = await client.Booking.PostAsync(booking);
            }
            catch(EduAdminApi.EduAdminException<ForbiddenResponse> ex)
            {
                var errorResult = ex.Result;
                var errorStr = "";

                foreach (var item in errorResult.Errors)
                {
                    //If multiple errors would exist, separate these
                    if (!string.IsNullOrWhiteSpace(errorStr))
                        errorStr += "<br />";

                    switch (item?.ErrorCode ?? 0)
                    {
                        case 40:
                            //No seats left
                            errorStr += _localizer["NotEnoughSeatsLeft"];
                            break;
                        case 45:
                            //Person already booked
                            errorStr += _localizer["BookingContainsBookedParticipants"];
                            break;
                        case 200:
                            //Person exists on overlapping sessions
                            errorStr += _localizer["BookingContainsOverlapPersons"];
                            break;
                        default:
                            errorStr += _localizer["CouldntCreateBookingTryAgain"];
                            break;
                    }
                }

                ModelState.AddModelError("", errorStr);
                return GetJsonResultFromModelStateErrors();
            }

            return Json(new CreateBookingResult
            {
                Success = true,
                SuccessRedirectUrl = Url.Action("BookingCreated", "Booking", new { eclID = newBooking.BookingId })
            });
        }

        public CreateBookingModel CreateBookingModelWithoutInfoQuestions(CreateBookingModel createBookingModel)
        {
            var bookingQuestions = new List<QuestionAnswer>();

            foreach (var bookingQuestion in createBookingModel.BookingQuestions)
            {
                if (!IsNonAnswerQuestionType(bookingQuestion))
                    bookingQuestions.Add(bookingQuestion);
            }

            createBookingModel.BookingQuestions = bookingQuestions;

            return createBookingModel;
        }

        private static bool IsNonAnswerQuestionType(QuestionAnswer bookingQuestion)
        {
            return IsInfoTextQuestions(bookingQuestion) || IsLineBreak(bookingQuestion) || IsNewParapgraph(bookingQuestion);
        }

        private static bool IsNewParapgraph(QuestionAnswer bookingQuestion)
        {
            return bookingQuestion.QuestionTypeID == Constants.QuestionTypes.NewParagraph;
        }

        private static bool IsLineBreak(QuestionAnswer bookingQuestion)
        {
            return bookingQuestion.QuestionTypeID == Constants.QuestionTypes.Linebreak;
        }

        private static bool IsInfoTextQuestions(QuestionAnswer bookingQuestion)
        {
            return bookingQuestion.QuestionTypeID == Constants.QuestionTypes.InfoText || bookingQuestion.QuestionTypeID == Constants.QuestionTypes.Infotext;
        }

        public ActionResult GetParticipant(int courseTemplateId, bool reqCivRegNumber)
        {
            return PartialView("../Booking/EditorTemplates/_Participant", new Models.Participant { RequireCivicRegistrationNumber = reqCivRegNumber });
        }

        public ActionResult BookingCreated(int? eclID)
        {
            var model = new BookingCreatedModel
            {
                ContactUsEmail = "support@legaonline.se"
            };

            return View("BookingCreated", model);
        }
    }
}