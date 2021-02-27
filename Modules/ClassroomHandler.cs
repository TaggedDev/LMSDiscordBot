using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Bot.Modules
{
    class ClassroomHandler
    {
        public static List<Course> courseArray = new List<Course>();
        public static string[] Scopes = { ClassroomService.Scope.ClassroomCoursesReadonly };
        public static string ApplicationName = "Quickstart";
        private static UserCredential credential;

        public static void ConnectClassroom()
        {
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Classroom API service.
            var service = new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            CoursesResource.ListRequest request = service.Courses.List();
            request.PageSize = 10;

            // List courses in courseArray.
            ListCoursesResponse response = request.Execute();
            CourseCount(response);

        }

        private static void UpdateClassroom()
        {
            var service = new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            CoursesResource.ListRequest request = service.Courses.List();
            request.PageSize = 10;

            // List courses in courseArray.
            ListCoursesResponse response = request.Execute();
            CourseCount(response);
        }

        private static void CourseCount(ListCoursesResponse response)
        {
            courseArray.Clear();
            if (response.Courses != null && response.Courses.Count > 0)
            {
                foreach (var course in response.Courses)
                {
                    courseArray.Add(course);
                }
            }
        }

        private static Course findCourseById(string id)
        {
            Course foundCourse = courseArray[0];
            foreach (Course myCourse in courseArray)
            {
                if (myCourse.Id == id)
                {
                    foundCourse = myCourse;
                }
            }
            return foundCourse;
        }

        public static List<Course> GetCourses()
        {
            UpdateClassroom();
            return courseArray;
        }

        public static Course ClassInformation(string id)
        {
            UpdateClassroom();
            return findCourseById(id);
        }
    }
}
