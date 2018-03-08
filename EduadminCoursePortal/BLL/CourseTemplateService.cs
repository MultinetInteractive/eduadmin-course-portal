using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EduAdminApi;
using EduAdminAPIClient.Models;
using EduadminCoursePortal.Models;
using Category = EduAdminApi.Category;
using Client = EduAdminAPIClient.Client;

namespace EduadminCoursePortal.BLL
{
    public interface ICourseTemplateService
    {
        Task<CourseTemplatesModel> GetCourseTemplates(int? subjectId = null, int? categoryId = null);
        Task<CourseTemplateModel> GetCourseTemplate(int courseTemplateId);
        Task<InterestRegistrationBasicCreated> CreateInterestReg(InterestRegModel model);
    }
    public class CourseTemplateService : ICourseTemplateService
    {
        private readonly IToken _token;

        public CourseTemplateService(IToken token)
        {
            _token = token;
        }

        public async Task<CourseTemplatesModel> GetCourseTemplates(int? subjectId = null, int? categoryId = null)
        {
            var newToken = await _token.GetNewToken();
            var client = new Client(newToken);

            var lastAppDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var startDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ");

            var courseTemplateQuery =
                $@"?$select=CourseTemplateId,CourseName,StartTime,EndTime,CourseDescriptionShort&$expand=Categories($select=CategoryId,CategoryName),Subjects($select=SubjectId,SubjectName),Events(
                    $select=AddressName,City,EventId,ParticipantNumberLeft,LastApplicationDate,MaxParticipantNumber,StartDate,EndDate,StatusId,StatusText,NumberOfBookedParticipants;
                    $expand=EventDates($select=StartDate,EndDate);
                    $filter=StatusId ne 3 and 
                    (ParticipantNumberLeft gt 0 or MaxParticipantNumber eq null) and 
                    LastApplicationDate gt {lastAppDate} and StartDate gt {startDate} and HasPublicPriceName eq true)
                &$filter=ShowOnWeb eq true and 
                    Events/any(e1:e1/StatusId ne 3 and 
                        e1/LastApplicationDate gt {lastAppDate} and 
                        e1/StartDate gt {startDate} and 
                        e1/HasPublicPriceName eq true and
                        (e1/ParticipantNumberLeft gt 0 or e1/MaxParticipantNumber eq null)
                    )" +
                (subjectId != null && subjectId > 0 ? $@" and Subjects/any(s:s/SubjectId eq {subjectId})" : "") +
                (categoryId != null && categoryId > 0 ? $@" and Categories/any(c:c/CategoryId eq {categoryId})" : "");

            var courseTemplates = await client.CourseTemplate.GetAsync(courseTemplateQuery);

            return new CourseTemplatesModel
            {
                CourseTemplatesWithEvents = GetCourseTemplateEvents(courseTemplates.Value),
                Subjects = GetCourseTemplateSubjects(courseTemplates.Value),
                Categories = GetCourseTemplateCategories(courseTemplates.Value)
            };
        }

        private List<CourseTemplateWithEvents> GetCourseTemplateEvents(List<CourseTemplate> educations)
        {
            return educations?.Select(x => new CourseTemplateWithEvents
            {
                CourseTemplateId = x.CourseTemplateId ?? 0,
                ObjectName = x.CourseName,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                CourseDescriptionShort = x.CourseDescriptionShort,
                Events = CreateEventModelList(x.Events.ToList(), x.CourseTemplateId ?? 0)
            }).ToList();
        }

        public async Task<CourseTemplateModel> GetCourseTemplate(int courseTemplateId)
        {
            var newToken = await _token.GetNewToken();
            var client = new Client(newToken);

            var lastAppDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var startDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ");

            var courseTemplateQuery =
                $@"?$select=CourseDescriptionShort,InternalCourseName,CourseDescription,CourseGoal,CourseName,CourseTemplateId,ImageUrl,TargetGroup&$expand=Events(
                        $select=AddressName,City,EventId,ParticipantNumberLeft,LastApplicationDate,MaxParticipantNumber,StartDate,EndDate,StatusId,StatusText,NumberOfBookedParticipants;
                        $expand=EventDates($select=StartDate,EndDate);
                        $filter=StatusId ne 3 and 
                            (ParticipantNumberLeft gt 0 or MaxParticipantNumber eq null) and 
                            LastApplicationDate gt {lastAppDate} and 
                            StartDate gt {startDate} and HasPublicPriceName eq true)
                    &$filter=ShowOnWeb eq true";

            var courseTemplate = await client.CourseTemplate.GetSingleAsync(courseTemplateId, courseTemplateQuery);

            return new CourseTemplateModel
            {
                CourseDescriptionShort = courseTemplate.CourseDescriptionShort,
                InternalCourseName = courseTemplate.InternalCourseName,
                CourseDescription = courseTemplate.CourseDescription,
                CourseGoal = courseTemplate.CourseGoal,
                EventCities = CreateEventCitiesList(courseTemplate.Events, courseTemplate.CourseTemplateId ?? 0),
                CourseName = courseTemplate.CourseName,
                CourseTemplateId = courseTemplate.CourseTemplateId ?? 0,
                ImageUrl = courseTemplate.ImageUrl,
                TargetGroup = courseTemplate.TargetGroup
            };
        }

        public List<EventCity> CreateEventCitiesList(List<CourseTemplateEvent> events, int courseTemplateId)
        {
            return events.GroupBy(x => x.City).Select(eventByCity => new EventCity
            {
                City = eventByCity.Key,
                Events = CreateEventModelList(eventByCity.ToList(), courseTemplateId)
            }).ToList();
        }

        public async Task<InterestRegistrationBasicCreated> CreateInterestReg(InterestRegModel model)
        {
            var newToken = await _token.GetNewToken();
            var client = new Client(newToken);

            try
            {
                var createInterestReg = await client.InterestRegistration.PostBasic(new PostInterestRegistrationBasic
                {
                    CompanyName = model.CompanyName,
                    CourseTemplateId = model.CourseTemplateId,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Mobile = model.Mobile,
                    Notes = model.Notes,
                    NumberOfParticipants = model.PartNr
                });

                return createInterestReg;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static List<Models.Category> GetCourseTemplateCategories(List<CourseTemplate> educationsWithEvents)
        {
            var categories = new List<Models.Category>();

            educationsWithEvents.ForEach(x => x.Categories.ForEach(y => categories.Add(new Models.Category
            {
                CategoryId = y.CategoryId ?? 0,
                CategoryName = y.CategoryName
            })));

            return categories.ToList();
        }

        private static List<Models.Subject> GetCourseTemplateSubjects(List<CourseTemplate> educationsWithEvents)
        {
            var subjects = new List<Models.Subject>();

            educationsWithEvents.ForEach(x => x.Subjects.ForEach(y => subjects.Add(new Models.Subject
            {
                SubjectName = y.SubjectName,
                SubjectID = y.SubjectId ?? 0
            })));

            return subjects.ToList();
        }

        private List<EventModel> CreateEventModelList(List<CourseTemplateEvent> events, int courseTemplateId)
        {
            return events.Select(item => new EventModel
            {
                AddressName = item.AddressName,
                City = item.City,
                EventDates = CreateEventDates(item.EventDates.ToList()),
                EventId = item.EventId ?? 0,
                IsFullyBooked = item.ParticipantNumberLeft != null && item.ParticipantNumberLeft <= 0,
                LastApplicationDate = item.LastApplicationDate?.DateTime ?? new DateTime(),
                MaxParticipantNr = item.MaxParticipantNumber,
                CourseTemplateId = courseTemplateId,
                ParticipantNrLeft = item.ParticipantNumberLeft,
                StartDate = item.StartDate?.DateTime ?? new DateTime(),
                EndDate = item.EndDate?.DateTime ?? new DateTime(),
                StatusId = item.StatusId ?? 0,
                StatusText = item.StatusText,
                TotalParticipantNr = item.NumberOfBookedParticipants ?? 0
            }).ToList();
        }

        private List<EventDates> CreateEventDates(List<EventDate> eventDates)
        {
            if (eventDates.Count <= 1)
                return null;

            return eventDates.Select(item => new EventDates
            {
                StartDateForEvent = item.StartDate?.DateTime ?? new DateTime(),
                EndDateForEvent = item.EndDate?.DateTime ?? new DateTime()
            }).ToList();
        }
    }
}
