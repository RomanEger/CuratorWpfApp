using CuratorWpfApp.Models.Enitities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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
                var list = await db.QueryAsync<Students>($"SELECT * FROM StudentsT WHERE Group_Name='{groupName}'");
                return list;
            }
        }

        public async Task<Students> GetStudentByIdAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryFirstOrDefaultAsync<Students>($"SELECT * FROM StudentsT WHERE Id={id}") ??
                    throw new Exception("Студент не найден");
            }
        }

        public async Task<int> AddStudentAsync(Students student, string s)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                int mil = Convert.ToInt32(student.Has_Military_id);
                return await db.ExecuteAsync("INSERT StudentsT " +
                    $"VALUES ('{student.Full_name}', '{student.Birthday}', '{student.Group_name}', {s}, {mil})");
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
                    $" Group_name='{student.Group_name}', Has_Military_id={mil}" +
                    $"WHERE Id={student.Id}");

                return await db.ExecuteAsync("UPDATE StudentsT " +
                    $"SET Full_name='{student.Full_name}', Birthday='{student.Birthday}'," +
                    $" Group_name='{student.Group_name}', Photo={photo}, Has_Military_id={mil}" +
                    $"WHERE Id={student.Id}");
            }
        }

        public async Task<int> DeleteStudentAsync(int id)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                return await db.ExecuteAsync($"DELETE StudentsT WHERE Id={id}");
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

        public async Task<IEnumerable<Debt>> GetDebtByIStudentdAsync(int idStudent, int semester)
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

        public async Task<IEnumerable<Debt>> GetDebtByGroupAsync(string groupName)
        {
            using( IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryAsync<Debt>("SELECT a.Name AS DisciplineName, s.Full_name AS StudentFullName, COUNT(Grade) AS CountDebts " +
                    "FROM GradesT AS g " +
                    "JOIN StudentsT AS s " +
                    "ON s.Id=g.Student_id " +
                    "JOIN Academic_disciplineT AS a " +
                    "ON a.Id=g.Discipline_id " +
                    $"WHERE Grade <= 2 AND s.Group_name=a.Group_name AND s.Group_name='{groupName}' " +
                    "GROUP BY GROUPING SETS((s.Full_name, a.Name)) ");
            }
        }
        public class View
        {
            public string Full_name { get; set; }
            public string Grades { get; set; }
        }
        public async Task<IEnumerable<View>> GetJournalAsync(string groupName, int disciplineId, int semester)
        {
            using(IDbConnection db = new SqlConnection(conStr))
            {
                return await db.QueryAsync<View>(
                    "select " +
                    "z.Full_name, " +
                    "(SELECT Grade FROM GradesT " +
                    $"WHERE Discipline_id = {disciplineId} AND Student_id = z.Id AND Semester = {semester}" +
                    $"FOR xml path('')) 'Grades' " +
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
    }
}
