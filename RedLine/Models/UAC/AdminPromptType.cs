namespace RedLine.Models.UAC
{
  public enum AdminPromptType : uint
  {
    AllowAll,
    DimmedPromptWithPasswordConfirmation,
    DimmedPrompt,
    PromptWithPasswordConfirmation,
    Prompt,
    DimmedPromptForNonWindowsBinaries,
  }
}
