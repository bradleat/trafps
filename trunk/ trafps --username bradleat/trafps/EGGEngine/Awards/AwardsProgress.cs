namespace EGGEngine.Awards
{
   /// <summary>
   /// Records a gamer's progress toward unlocking an award.
   /// </summary>
   public class AwardProgress
   {
      /// <summary>
      /// Gets or sets the award that is being unlocked.
      /// </summary>
      public Award Award { get; set; }
      
      /// <summary>
      /// Gets or sets the gamertag of the gamer unlocking the award.
      /// </summary>
      public string Gamertag { get; set; }
      
      /// <summary>
      /// Gets or sets the progress points acquired for the award.
      /// </summary>
      public int Progress { get; set; }
      
      /// <summary>
      /// Returns true if the award is unlocked by the gamer, false otherwise.
      /// </summary>
      public bool IsUnlocked
      {
         get
         {
            return Progress >= Award.ProgressNeeded;
         }
      }
   }
}