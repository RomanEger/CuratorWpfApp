﻿using CuratorWpfApp.Models.Enitities;
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
                var grades = await db.QueryAsync<int>($"SELECT Grade FROM GradesT WHERE Student_id={idStudent} AND Semester={semester}");
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
                    $"SELECT DISTINCT(Academic_disciplineT.Name) " +
                    $"FROM GradesT " +
                    $"JOIN Academic_disciplineT " +
                    $"ON GradesT.Discipline_id = Academic_disciplineT.Id " +
                    $"WHERE Student_id={idStudent} AND Grade<=2 AND Semester={semester}");
                List<int> count = new();
                foreach(var item in name)
                {
                    count.Add(await db.QueryFirstAsync<int>(
                        $"SELECT COUNT(Grade) " +
                        $"FROM GradesT " +
                        $"JOIN Academic_disciplineT " +
                        $"ON GradesT.Discipline_id = Academic_disciplineT.Id " +
                        $"WHERE Student_id={idStudent} AND Grade<=2 AND Academic_disciplineT.Name='{item}' AND Semester={semester}"));
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

    }
}
