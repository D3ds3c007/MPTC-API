
namespace MPTC_API.Services.Education.UtilClasses;
public class Section
{
    public List<Question> Questions { get; set; }
    public double Score { get; set; }
    public double Scale { get; set; }
    public string Name { get; set; }

    public Section(){
        this.Name = "";
        this.Questions = new List<Question>();
    }
}
