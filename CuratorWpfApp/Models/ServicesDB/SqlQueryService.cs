using CuratorWpfApp.Models.Enitities;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace CuratorWpfApp.Models.ServicesDB
{
    class SqlQueryService
    {
        private string conStr = ConfigStr.ConStr;

        public async Task<Users?> LoginAsync(string login, string password)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<Users?>($"SELECT * FROM UsersT WHERE Login='{login}' AND Password='{password}'")
                    ?? throw new Exception("Некорректный логин или пароль");
            }
        }

        public async Task<IEnumerable<Students>> GetStudentsByGroupAsync(string groupName)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                var list = await db.QueryAsync<Students>($"SELECT * FROM StudentsT WHERE Group_Name='{groupName}' AND IsExpelled=0");
                return list;
            }
        }

        public async Task<Students> GetStudentByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<Students>($"SELECT * FROM StudentsT WHERE Id={id} AND IsExpelled=0") ??
                    throw new Exception("Студент не найден");
            }
        }

        public async Task<int> AddStudentAsync(Students student, string s)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                int mil = Convert.ToInt32(student.Has_Military_id);
                return await db.ExecuteAsync("INSERT StudentsT " +
                    $"VALUES ('{student.Full_name}', '{student.Birthday}', '{student.Group_name}', {s}, {mil}, 0)");
            }
        }

        public async Task<int> UpdateStudentAsync(Students student, string photo)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                int mil = Convert.ToInt32(student.Has_Military_id);

                if (string.IsNullOrEmpty(photo))
                    return await db.ExecuteAsync("UPDATE StudentsT " +
                    $"SET Full_name='{student.Full_name}', Birthday='{student.Birthday}'," +
                    $" Group_name='{student.Group_name}', Has_Military_id={mil}, IsExpelled={student.IsExpelled}" +
                    $"WHERE Id={student.Id}");

                return await db.ExecuteAsync("UPDATE StudentsT " +
                    $"SET Full_name='{student.Full_name}', Birthday='{student.Birthday}'," +
                    $" Group_name='{student.Group_name}', Photo={photo}, Has_Military_id={mil}, IsExpelled={student.IsExpelled}" +
                    $"WHERE Id={student.Id}");
            }
        }

        /// <summary>
        /// МЕТОД НЕ ГОТОВ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<int> ChangeStudentStatusAsync(int id, string groupName, string description, string dateTime)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.ExecuteAsync(
                    $"UPDATE StudentsT " +
                    $"SET ..." +
                    $"WHERE Id={id}");
            } 
        }

        public async Task<IEnumerable<string>> GetDisciplinesAsync(string groupName)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryAsync<string>($"SELECT 'Название: ' + Name + CHAR(13)+CHAR(10) + 'Преподаватель: ' + Teacher_full_name AS someName FROM Academic_disciplineT WHERE Group_name='{groupName}'");
            }
        }

        public async Task<IEnumerable<int>> GetGradesByStudentAndDiscipline(int idStudent, string disciplineName, string teacherFullName, int semester)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                int idDiscipline = await db.QueryFirstOrDefaultAsync<int>(
                    $"SELECT Id FROM Academic_disciplineT " +
                    $"WHERE Teacher_full_name='{teacherFullName}' AND Name='{disciplineName}' ");
                return await db.QueryAsync<int>($"SELECT Grade FROM GradesT WHERE Discipline_id={idDiscipline} AND Student_id={idStudent} AND Semester={semester}");
            }
        }

        public async Task<double> GetAvgGradeByStudent(int idStudent, int semester)
        {
            using( IDbConnection db = new SqlConnection(conStr))
            {
                var grades = await db.QueryAsync<int>(
                    $"SELECT Grade FROM GradesT " +
                    $"JOIN StudentsT AS s " +
                    $"ON GradesT.Student_id=s.Id " +
                    $"JOIN Academic_disciplineT AS a " +
                    $"ON GradesT.Discipline_id=a.Id " +
                    $"WHERE Student_id={idStudent} AND Semester={semester} AND a.Group_name=s.Group_name");
                double avgGrade = Math.Round(grades.Average(),3);
                return avgGrade;
            }
        }

        public async Task<IEnumerable<Debt>> GetDebtByIStudentIdAsync(int idStudent, int semester)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                List<Debt> debt = new();
                var name = await db.QueryAsync<string>(
                    $"SELECT DISTINCT(a.Name) " +
                    $"FROM GradesT " +
                    $"JOIN Academic_disciplineT AS a " +
                    $"ON GradesT.Discipline_id = a.Id " +
                    $"JOIN StudentsT AS s " +
                    $"ON GradesT.Student_id=s.Id " +
                    $"WHERE Student_id={idStudent} AND Grade<=2 AND Semester={semester} AND s.Group_name=a.Group_name");
                List<int> count = new();
                foreach(var item in name)
                {
                    count.Add(await db.QueryFirstAsync<int>(
                        $"SELECT COUNT(Grade) " +
                        $"FROM GradesT " +
                        $"JOIN Academic_disciplineT AS a " +
                        $"ON GradesT.Discipline_id = a.Id " +
                        $"JOIN StudentsT AS s " +
                        $"ON GradesT.Student_id=s.Id " +
                        $"WHERE Student_id={idStudent} AND Grade<=2 AND a.Name='{item}' AND Semester={semester} AND s.Group_name=a.Group_name"));
                }
                var nameArr = name.ToArray();
                for(int i = 0; i < nameArr.Length; i++)
                    debt.Add(new Debt() { DisciplineName = nameArr[i], CountDebts = count[i] });
                return debt;
            }
        }

        public async Task<IEnumerable<Certificates>> GetCertificatesByStudentIdAsync(int idStudent)
        {
            using( IDbConnection db = new SqlConnection( conStr ))
                return await db.QueryAsync<Certificates>($"SELECT * FROM CertificatesT WHERE Student_id={idStudent}");
        }

        public async Task<IEnumerable<CertificatesTable>> GetCertificatesByGroupAsync(string groupName)
        {
            using(IDbConnection db = new SqlConnection( conStr ))
            {
                var l = await db.QueryAsync<int>($"SELECT Id FROM StudentsT WHERE Group_name='{groupName}'");
                var certificates = new List<CertificatesTable>();
                foreach(var item in l)
                {
                    certificates.AddRange(await db.QueryAsync<CertificatesTable>(
                        $"SELECT CertificatesT.Id, StudentsT.Full_name, CertificatesT.Title, " +
                        $"CertificatesT.Start_date, CertificatesT.End_date FROM CertificatesT " +
                        $"JOIN StudentsT " +
                        $"ON CertificatesT.Student_id=StudentsT.Id " +
                        $"WHERE Student_id={item}"));
                }
                var result = certificates.OrderBy(x=> x.Id);
                return result;
            }
        }

        public async Task<Certificates> GetCertificateByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(conStr))
                return await db.QueryFirstOrDefaultAsync<Certificates>($"SELECT * FROM CertificatesT WHERE Id={id}") ??
                    throw new Exception("Справка не найдена");
        }

        public async Task<int> DeleteCertificateAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.ExecuteAsync($"DELETE CertificatesT WHERE Id={id}");
            }

        }

        public async Task<int> UpdateCertificateAsync(Certificates certificate)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                return await db.ExecuteAsync("UPDATE CertificatesT " +
                    $"SET Title='{certificate.Title}', Start_date='{certificate.Start_date}', End_date='{certificate.End_date}' " +
                    $"WHERE Id={certificate.Id}");
            }
        }

        public async Task<int> AddCertificateAsync(Certificates certificate)
        {
            using(IDbConnection db = new SqlConnection( conStr))
            {
                return await db.ExecuteAsync("INSERT CertificatesT " +
                    $"VALUES ({certificate.Student_id}, '{certificate.Title}', '{certificate.Start_date}', '{certificate.End_date}')");
            }
        }

        public async Task<IEnumerable<Debt>> GetDebtByGroupAsync(string groupName, int semester)
        {
            using( IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryAsync<Debt>("SELECT a.Name AS DisciplineName, s.Full_name AS StudentFullName, COUNT(Grade) AS CountDebts " +
                    "FROM GradesT AS g " +
                    "JOIN StudentsT AS s " +
                    "ON s.Id=g.Student_id " +
                    "JOIN Academic_disciplineT AS a " +
                    "ON a.Id=g.Discipline_id " +
                    $"WHERE Grade <= 2 AND s.Group_name=a.Group_name AND s.Group_name='{groupName}' AND g.Semester={semester} " +
                    "GROUP BY GROUPING SETS((s.Full_name, a.Name)) ");
            }
        }
        public class View
        {
            public string Full_name { get; set; }
            public string Grades { get; set; }
            public string Id { get; set; }
            private string dateL = string.Empty;
            public string DateL
            {
                get => dateL.Replace(" 00:00:00", "");
                set => dateL = value;
            }
        }
        public async Task<IEnumerable<View>> GetJournalAsync(string groupName, int disciplineId, int semester)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryAsync<View>(
                    "SELECT " +
                    "z.Full_name, " +
                    "(SELECT Grade FROM GradesT " +
                    $"WHERE Discipline_id = {disciplineId} AND Student_id = z.Id AND Semester = {semester}" +
                    $"FOR xml path('')) AS 'Grades', " +
                    "(SELECT Id FROM GradesT " +
                    $"WHERE Discipline_id = {disciplineId} AND Student_id = z.Id AND Semester = {semester}" +
                    $"FOR xml path('')) AS 'Id', " +
                    "(SELECT DateL FROM DateLessonsT " +
					"JOIN GradesT "+
					"ON GradesT.StudyDateId=DateLessonsT.Id "+
                    $"WHERE Discipline_id = {disciplineId} AND Student_id = z.Id AND Semester = {semester} "+
                    "FOR xml path('')) AS 'DateL' "+
                    $"from StudentsT z " +
                    $"inner join GradesT n " +
                    $"on n.Student_id = z.Id " +
                    $"group by z.Full_name, z.Id");
            }
        }

        public async Task<int> GetIdAcademicDisciplinesAsync(string groupName, string title, string fullName)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                var q = "SELECT Id FROM Academic_disciplineT " +
                    $"WHERE Group_name='{groupName}' AND Name='{title}' AND Teacher_full_name='{fullName}' ";
                return await db.QueryFirstAsync<int>(
                    q);
            }
        }

        public async Task<string> GetCuratorFullNameAsync(string groupName)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<string>($"SELECT Full_name FROM UsersT WHERE Group_name='{groupName}' AND Role_id=1") ??
                    throw new Exception($"Куратор группы {groupName} не найден");
            }
        }

        public async Task<IEnumerable<Students>> GetStudentsByGroupAsOf1SeptemberAsync(string groupName)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryAsync<Students>($"SELECT * FROM StudentsAsOf1SeptemberT WHERE Group_name='{groupName}'");
            }
        }

        public async Task<IEnumerable<Students>> GetStudentsWithoutDebtsByGroupAsync(string groupName, int semester)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                var q = await db.QueryAsync<int>(
                    $"SELECT DISTINCT Student_id FROM GradesT " +
                    $"WHERE Semester={semester} " +
                    $"GROUP BY Student_id " +
                    $"HAVING MIN(Grade) < 3");

                var sb = new StringBuilder();
                sb.Append("SELECT * FROM StudentsT " +
                          $"WHERE Group_name='{groupName}' ");

                if (q.Any())
                {
                    for (int i = 0; i < q.Count(); i++)
                    {
                        sb.Append($" AND Id <> {q.AsList()[i]}");
                    }

                }

                return await db.QueryAsync<Students>(sb.ToString());
            }
        }

        public async Task<IEnumerable<string>> GetStudentsByAvgGradesAsync(string groupName, int semester, int course, double minAvgGrade, double maxAvgGrade = 5.1)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                string minAvg = minAvgGrade.ToString().Replace(',', '.');
                string maxAvg = maxAvgGrade.ToString().Replace(',', '.');
                var q = 
                    "SELECT distinct Student_id FROM Final_GradesT " +
                    "JOIN StudentsT AS S " +
                    "ON S.Id=Final_GradesT.Student_id " +
                    $"WHERE Group_name='{groupName}' AND Semester={semester} AND Course={course} AND Grade < 4 " +
                    "GROUP BY Discipline_id, Student_id, Group_name " +
                    $"HAVING AVG(Grade * 1.0) <= {minAvg}";
                
                var list = await db.QueryAsync<int>(q);

                var sb = new StringBuilder("SELECT DISTINCT Full_name FROM Final_GradesT " +
                                           "JOIN StudentsT AS S " +
                                           "ON S.Id=Final_GradesT.Student_id ");
                var enumerable = list as int[] ?? list.ToArray();
                if (enumerable.Any())
                {
                    sb.Append($"WHERE Student_id <> {enumerable[0]} ");
                    for (int i = 1; i < enumerable.Length; i++)
                    {
                        sb.Append($"AND Student_id <> {enumerable[0]} ");
                    }
                }
                
                sb.Append("GROUP BY Full_name " +
                          $"HAVING AVG(Grade * 1.0) >= {minAvg} AND AVG(Grade * 1.0) < {maxAvg}");


                return await db.QueryAsync<string>(sb.ToString());
            }
        }

        public async Task<int> GetNiceProcent(string groupName, int semester, int course)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                var q =
                    "DECLARE @countStd DECIMAL, @count3 DECIMAL, @count4 DECIMAL, @count5 DECIMAL, @niceProcent DECIMAL;" +

                    "SET @count3 = (SELECT COUNT(Grade) FROM Final_GradesT " +
                    "JOIN StudentsT " +
                    "ON StudentsT.Id=Final_GradesT.Student_id " +
                    $"WHERE Grade=3 AND Semester={semester} AND Course={course} AND StudentsT.Group_name='{groupName}');" +

                    "SET @count4 = (SELECT COUNT(Grade) FROM Final_GradesT " +
                    "JOIN StudentsT " +
                    "ON StudentsT.Id=Final_GradesT.Student_id " +
                    $"WHERE Grade=4 AND Semester={semester} AND Course={course} AND StudentsT.Group_name='{groupName}');" +

                    "SET @count5 = (SELECT COUNT(Grade) FROM Final_GradesT " +
                    "JOIN StudentsT " +
                    "ON StudentsT.Id=Final_GradesT.Student_id " +
                    $"WHERE Grade=5 AND Semester={semester} AND Course={course} AND StudentsT.Group_name='{groupName}');" +

                    "SET @countStd = (SELECT COUNT(Id) FROM StudentsT " +
                    $"WHERE Group_name='{groupName}');" +

                    "SET @niceProcent = ((@count3+@count4+@count5)/@countStd)*100;" +

                    "SELECT @niceProcent;";
                return await db.QueryFirstOrDefaultAsync<int>(q);
            }
        }
        
        public async Task<int> GetGoodProcent(string groupName, int semester, int course)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                var q =
                    "DECLARE @countStd DECIMAL, @count4 DECIMAL, @count5 DECIMAL, @goodProcent DECIMAL;" +

                    "SET @count4 = (SELECT COUNT(Grade) FROM Final_GradesT " +
                    "JOIN StudentsT " +
                    "ON StudentsT.Id=Final_GradesT.Student_id " +
                    $"WHERE Grade=4 AND Semester={semester} AND Course={course} AND StudentsT.Group_name='{groupName}');" +

                    "SET @count5 = (SELECT COUNT(Grade) FROM Final_GradesT " +
                    "JOIN StudentsT " +
                    "ON StudentsT.Id=Final_GradesT.Student_id " +
                    $"WHERE Grade=5 AND Semester={semester} AND Course={course} AND StudentsT.Group_name='{groupName}');" +

                    "SET @countStd = (SELECT COUNT(Id) FROM StudentsT " +
                    $"WHERE Group_name='{groupName}');" +

                    "SET @goodProcent = ((@count4+@count5)/@countStd)*100;" +

                    "SELECT @goodProcent;";
                return await db.QueryFirstOrDefaultAsync<int>(q);
            }
        }

        public async Task<IEnumerable<DateLessons>> GetDateLessons()
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryAsync<DateLessons>("SELECT * FROM DateLessonsT");
            }
        }

        public async Task<IEnumerable<DateLessons>> AddNewDateLesson(string ss)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                await db.ExecuteAsync("INSERT DateLessonsT " +
                                      $"VALUES ('{ss}')");
                return await db.QueryAsync<DateLessons>("SELECT TOP 1 * FROM DateLessonsT " +
                                                        "ORDER BY Id DESC") ?? throw new Exception("Не удалось найти дату");
            }
        }

        public async Task<int?> GetGradeByFilters(int studentId, int disciplineId, int studyDateId)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<int?>("SELECT Grade FROM GradesT " +
                                                               $"WHERE Student_id={studentId} AND Discipline_id={disciplineId} AND StudyDateId={studyDateId}");
            }
        }

        public async Task AddOrUpdateGrade(int studentId, int disciplineId, int studyDateId, int semester, int grade)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                var a = await db.QueryFirstOrDefaultAsync<int?>(
                    "SELECT Grade FROM GradesT " +
                    $"WHERE Student_id={studentId} AND Discipline_id={disciplineId} AND StudyDateId={studyDateId} AND Semester={semester}");
                if (a == null)
                    await db.ExecuteAsync(
                        "INSERT GradesT " +
                            $"VALUES({studentId}, {disciplineId}, {grade}, {semester}, {studyDateId})");
                else
                    await db.ExecuteAsync(
                        "UPDATE GradesT " +
                            $"SET Grade={grade} " +
                            $"WHERE Student_id={studentId} AND Discipline_id={disciplineId} AND StudyDateId={studyDateId} AND Semester={semester}");
            }
        }
    }
}
