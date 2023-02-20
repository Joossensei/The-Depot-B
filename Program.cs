namespace Test {
    public class Program
    {
        public static void Main(string[] args)
        {
            var input = Console.ReadLine();
            var input2 = Console.ReadLine();

            int int1 = 0;
            int int2 = 0;

            if (input != null && input2 != null){
                int1 = Int32.Parse(input);
                int2 = Int32.Parse(input2);
            }

            for (int i = int1; i < int2; i++)
            {
                string output = "";

                if (i % 3 == 0)
                {
                    output += "Fizz";
                }

                if (i % 5 == 0)
                {
                    output += "Buzz";
                }

                if(i % 5 != 0 && i % 3 != 0)
                {
                    output = i.ToString();
                }

                Console.WriteLine(output);
            }
        }
    }
}/* dit is een test*/