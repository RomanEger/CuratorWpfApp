using CuratorWpfApp.Models.ServicesDB;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using CuratorWpfApp.Models.Enitities;
using Word = Microsoft.Office.Interop.Word;

namespace CuratorWpfApp.Services
{
    partial class ReportSemester
    {
        
        public ReportSemester(string groupName, string fileName, string newPath)
        {
            this.groupName = groupName;

            if (File.Exists(fileName))
            {
                fileInfo = new FileInfo(fileName);
            }
            else
            {
                throw new Exception("Файл не найден");
            }

            Function(newPath);
        }

        private async void Function(string newPath)
        {
            await SetData();
            FillWordFile(out newPath);
        }

        private async Task SetData()
        {
            SqlQueryService sqlService = new();
            curator = await sqlService.GetCuratorFullNameAsync(groupName);
            if (DateTime.Now.Month < 9)
            {
                semester = 2;
                start = DateTime.Now.Year-1;
                end = DateTime.Now.Year;
            }
            else
            {
                semester = 1;
                start = DateTime.Now.Year;
                var s = start.ToString();
                var c = s.ToCharArray();
                c[^1] = '0';
                s = new string(c);
                int s1;
                int.TryParse(s, out s1);
                start -= s1; 

                end = DateTime.Now.Year+1;
                s = end.ToString();
                c = s.ToCharArray();
                c[^1] = '0';
                s = new string(c);
                int.TryParse(s, out s1);
                end -= s1;
            }
            //add try catch

            var list = (List<Debt>)await sqlService.GetDebtByGroupAsync(groupName, semester);

            if (list.Count > 0)
                badStdList = new List<BadStd>();
            foreach (var item in list)
            {
                badStdList.Add(new BadStd()
                {
                    FullName = item.StudentFullName,
                    DisciplineName = item.DisciplineName
                });
            }

            veryBadStdList = list.Where(x => x.CountDebts >= 3).ToList();

            bad = badStdList.Count;

            veryBad = veryBadStdList.Count;

            count1 = (await sqlService.GetStudentsByGroupAsOf1SeptemberAsync(groupName)).Count();
            countReport = (await sqlService.GetStudentsByGroupAsync(groupName)).Count();

            int course = 1;
            foreach (var item in groupName)
            {
                if (char.IsDigit(item))
                {
                    course = Convert.ToInt32(item.ToString());
                    break;
                }
            }

            nice = (await sqlService.GetStudentsWithoutDebtsByGroupAsync(groupName, semester)).Count();

            good = (await sqlService.GetStudentsByAvgGradesAsync(groupName, semester, course,4.0, 4.75)).Count();

            superStudentList = (await sqlService.GetStudentsByAvgGradesAsync(groupName, semester, course, 4.75)).ToList();

            super = superStudentList.Count;

            sumGoodAndSuper = good + super;

            niceProcent = await sqlService.GetNiceProcent(groupName, semester, course);

            goodProcent = await sqlService.GetGoodProcent(groupName, semester, course);
            dict = new Dictionary<string, string>()
            {
                {"<GROUPNAME>", groupName},
                {"<SEMESTER>", semester.ToString()},
                {"<START>", start.ToString()},
                {"<END>", end.ToString()},
                {"<COUNT1>", $"{count1}" },
                {"<COUNTREPORT>", $"{countReport}"},
                {"<NICE>", $"{nice}"},
                {"<NICEPROCENT>", $"{niceProcent}"},
                {"<SUPER>", super.ToString()},
                {"<GOOD>", good.ToString()},
                {"<SUMGOODANDSUPER>", sumGoodAndSuper.ToString()},
                {"<GOODPROCENT>", goodProcent.ToString()},
                {"<BAD>", bad.ToString()},
                {"<VERYBAD>", veryBad.ToString()}
            };
        }

        private bool FillWordFile(out string newPath)
        {
            string newFileName;
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Документ Word (*.docx)|*.docx|All files (*.*)|*.*",
                FileName = $"Отчет {curator} {groupName} заведующему отделением"
            };
            Word.Application word = new();
            if (saveFileDialog.ShowDialog() == true)
            {
                newFileName = saveFileDialog.FileName;
                try
                {
                    var file = fileInfo.FullName;

                    word.Documents.Open(file);

                    ChangeText(word);
                    WorkWithTable(1, word);
                    WorkWithTable(2, word);

                    WorkWithList(word);

                    word.ActiveDocument.SaveAs2(newFileName);

                    newPath = newFileName;


                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    newPath = null;
                    return false;
                }
                finally
                {
                    word.ActiveDocument.Close();
                    word.Quit();
                }
            }
            newPath = null;
            MessageBox.Show("Документ сохранен по адресу " +newPath);
            return false;
        }

        private void ChangeText(Word.Application word)
        {
            var missing = Type.Missing;

            foreach (var item in dict)
            {
                Word.Find find = word.Selection.Find;
                find.Text = item.Key;
                find.Replacement.Text = item.Value;

                var wrap = Word.WdFindWrap.wdFindContinue;
                var replace = Word.WdReplace.wdReplaceAll;

                find.Execute(
                    FindText: Type.Missing,
                    MatchCase: false,
                    MatchWholeWord: false,
                    MatchWildcards: false,
                    MatchSoundsLike: missing,
                    MatchAllWordForms: false,
                    Forward: true,
                    Wrap: wrap,
                    Format: false,
                    ReplaceWith: missing,
                    Replace: replace
                    );

            }

        }


        private void WorkWithList(Word.Application word)
        {
            Word.Paragraph lastPara = null;
            if (super == 0)
            { 
                Word.Find find = word.Selection.Find;
                find.Text = "Фамилии:";
                find.Replacement.Text = "";

                var wrap = Word.WdFindWrap.wdFindContinue;
                var replace = Word.WdReplace.wdReplaceAll;

                find.Execute(
                    FindText: Type.Missing,
                    MatchCase: false,
                    MatchWholeWord: false,
                    MatchWildcards: false,
                    MatchSoundsLike: Type.Missing,
                    MatchAllWordForms: false,
                    Forward: true,
                    Wrap: wrap,
                    Format: false,
                    ReplaceWith: Type.Missing,
                    Replace: replace
                );

            }
            foreach (Word.Paragraph para in word.ActiveDocument.Paragraphs)
            {
                if (para.Range.ListFormat.ListType != Word.WdListType.wdListSimpleNumbering) 
                    continue;
                para.Range.Text = $"";
                lastPara = para;
            }

            for (int i = 0; i < superStudentList.Count; i++)
            {
                if (lastPara == null) 
                    continue;
                lastPara = word.ActiveDocument.Paragraphs.Add(lastPara.Range);
                lastPara.Range.InsertParagraphAfter();
                lastPara.Range.Text = $"\t\t*\t{superStudentList[i]}";
            }

        }

        private void WorkWithTable(int index, Word.Application word)
        {
            if (index > word.ActiveDocument.Tables.Count)
                index = word.ActiveDocument.Tables.Count;
            Word.Table t = word.ActiveDocument.Tables[index];
            int i = t.Rows.Count;
            if (index == 1)
            {
                if (t.Rows.Count - 1 > bad)
                {

                    while (t.Rows.Count - 1 > bad)
                    {
                        if (--i == 2)
                            break;
                        t.Rows[t.Rows.Count]?.Delete();
                    }
                }
                else
                {
                    while (t.Rows.Count <= bad)
                        t.Rows.Add();

                }

                if (i > 0)
                {
                    int zz = 0;
                    int yy = 0;
                    foreach (Word.Row row in t.Rows)
                    {
                        foreach (Word.Cell cell in row.Cells)
                        {
                            if (cell.RowIndex > 1)
                            {
                                if (cell.ColumnIndex == 1)
                                {
                                    cell.Range.Text = badStdList[zz++].FullName;
                                }
                                else if (cell.ColumnIndex == 2)
                                {
                                    cell.Range.Text = badStdList[yy++].DisciplineName;
                                }
                            }
                        }

                    }
                }
            }
            else if (index == 2)
            {
                if (t.Rows.Count - 1 > veryBad)
                {
                    while (t.Rows.Count - 1 > veryBad)
                    {
                        t.Rows[t.Rows.Count]?.Delete();
                        if (--i == 2)
                            break;
                    }
                }
                else
                {
                    while (t.Rows.Count <= veryBad)
                        t.Rows.Add();

                }

                if (i > 0)
                {
                    int zz = 0;
                    int yy = 0;
                    int xx = 0;
                    foreach (Word.Row row in t.Rows)
                    {
                        foreach (Word.Cell cell in row.Cells)
                        {
                            if (cell.RowIndex > 1)
                            {
                                if (cell.ColumnIndex == 1)
                                {
                                    cell.Range.Text = veryBadStdList[xx++].StudentFullName;
                                }
                                else if (cell.ColumnIndex == 2)
                                {
                                    cell.Range.Text = veryBadStdList[yy++].CountDebts.ToString();
                                }
                                else if (cell.ColumnIndex == 3)
                                {
                                    cell.Range.Text = veryBadStdList[zz++].DisciplineName;
                                }
                                
                            }
                        }

                    }
                }
            }
        }
    }
}
