using System.IO;
using CuratorWpfApp.Models.Enitities;
using CuratorWpfApp.Models.ServicesDB;

namespace CuratorWpfApp.Services;

partial class ReportSemester
{
    private List<BadStd> badStdList;

    private List<Debt> veryBadStdList;

    private List<string> superStudentList;

    private string curator;

    private FileInfo fileInfo;

    /// <summary>
    /// Key - строка в файле, подлежащая замене;
    /// Value - новое значение
    /// </summary>
    private Dictionary<string, string> dict;

    private string groupName;

    private int semester = 1;

    /// <summary>
    /// начало учебного года
    /// </summary>
    private int start;

    /// <summary>
    /// конец у.г.
    /// </summary>
    private int end;

    /// <summary>
    /// кол-во студентов на 1 сент
    /// </summary>
    private int count1;

    /// <summary>
    /// кол-во студентов на отчетный период
    /// </summary>
    private int countReport;

    /// <summary>
    /// успевают . чел
    /// </summary>
    private int nice;

    /// <summary>
    /// процент успеваемости
    /// </summary>
    private int niceProcent;

    /// <summary>
    /// учатся на отлично
    /// </summary>
    private int super;
    
    /// <summary>
    /// учатся на хорошо и отлично
    /// </summary>
    private int good;

    /// <summary>
    /// super + good
    /// </summary>
    private int sumGoodAndSuper;

    /// <summary>
    /// процент качества успеваемости
    /// </summary>
    private int goodProcent;

    /// <summary>
    /// не успевают по 1-3 дисциплинам
    /// </summary>
    private int bad;

    /// <summary>
    /// не успевают по 3+ дисциплинам
    /// </summary>
    private int veryBad;
}