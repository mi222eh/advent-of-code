// See https://aka.ms/new-console-template for more information

var input = Common.ReadInputLinesDefault().ToList();

var recordList = new List<Record>();

input.ForEach(line =>
{
   if (string.IsNullOrEmpty(line)) return;
   Record record = new();
   line.Split(" ").Select(int.Parse).ToList().ForEach(nr => record.AddLevel(nr));
   recordList.Add(record);
});

var nrOfValid = recordList.Count(rec => rec.IsValid());


Console.WriteLine(nrOfValid);


class Record
{
   private static int MAXIMUM_DIFFERENCE = 3;
   
   private List<Level> Levels = [];
   public void AddLevel(int Value) => Levels.Add(new(Value));
   
   public bool IsValid ()
   {
      var isValid = AreLevelsValid(Levels);
      if (isValid) return true;
      var nrOfLevels = Levels.Count;
      var currentIndex = 0;
      while (currentIndex < nrOfLevels)
      {
         var list = Levels.ToList();
         list.RemoveAt(currentIndex);
         if (AreLevelsValid(list)) return true;
         currentIndex++;
      }

      return false;
   }
   
   public static bool AreLevelsValid (IEnumerable<Level> levels)
   {
      var levelList = levels.ToList();
      var diffOk = AreLevelsWithingMaxDifference(levelList);
      if (!diffOk) return false;
      
      var isInc = AreLevelsIncreasing(levelList);
      var isDesc = AreLevelsDecreasing(levelList);

      return isDesc || isInc;
   }
   public static bool AreLevelsIncreasing (IEnumerable<Level> levels) {
      var levelList = levels.ToList();
      var currentNumber = levelList.First().Value;
      return levelList.Skip(1).All(level =>
      {
         if (level.Value <= currentNumber) return false;
         currentNumber = level.Value;
         return true;
      });
   }
   public static bool AreLevelsDecreasing (IEnumerable<Level> levels) {
      var levelList = levels.ToList();
      var currentNumber = levelList.First().Value;
      return levelList.Skip(1).All(level =>
      {
         if (level.Value >= currentNumber) return false;
         currentNumber = level.Value;
         return true;
      });
   }
   public static bool AreLevelsWithingMaxDifference (IEnumerable<Level> levels)
   {
      var levelList = levels.ToList();
      int currentNumber = levelList.First().Value;
      return levelList.Skip(1).All(level =>
      {
         if (level.GetDifference(currentNumber) > MAXIMUM_DIFFERENCE) return false;
         currentNumber = level.Value;
         return true;
      });
   }
}

class Level(int _value)
{
   public int Value => _value;
   public int GetDifference(int iValue) => int.Abs(_value - iValue);
};

