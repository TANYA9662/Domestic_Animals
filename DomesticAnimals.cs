
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System;
using System.Reflection;
using System.Xml.Linq;


namespace MySQLDBConnection
{
	internal class DomesticAnimals
	{
		static void Main(string[] args)
		{


			//Iniitera variabeler deklaration
			string server = "LOCALHOST ";
			string database = "DomesticAnimals";
			string username = "root";
			string pass = " "; //Ange lösenord

			string strConn = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={pass};";

			//Establera koppling till Databas
			MySqlConnection conn = new MySqlConnection(strConn);

			ConsoleKeyInfo input;

			//Meny
			do
			{
				Console.Clear();

				//Skriva ut en meny för användaren
				Console.WriteLine("Välj ditt val för DB funktion!");
				Console.WriteLine("------------------------------");
				Console.WriteLine("1. Skriv data");
				Console.WriteLine("2. Hämta data");
				Console.WriteLine("3. Updatera data");
				Console.WriteLine("4. Ta bort data");
				Console.WriteLine("5. Avsluta");

				//Låta användaren välja ett alternativ
				input = Console.ReadKey();

				//Ta värdet till en SwitchCase
				switch (input.KeyChar.ToString())
				{
					case "1":
						Console.Clear();
						WriteData(conn);
						break;
					case "2":
						Console.Clear();
						FetchData(conn);
						break;
					case "3":
						Console.Clear();
						UpdateData2(conn);
						break;
					case "4":
						Console.Clear();
						RemoveData(conn);
						break;
					case "5":
						break;
					default:

						Console.WriteLine("Du har matat in ett felaktigt värde. (Press any key to continue...)");
						Console.ReadKey();
						break;

				}
			} while (input.KeyChar.ToString() != "5");


		}

		public static void WriteData(MySqlConnection conn)
		{
			//Hämta data från användare
			Console.Write("Type the animal_name: ");
			string name = Console.ReadLine();

			Console.Write("Type the animal_weight: ");
			double weight = Convert.ToDouble(Console.ReadLine());


			Console.Write("Type the animal_age: ");
			int age = Convert.ToInt32(Console.ReadLine());


			// SQL Querry för INSERT
			string sqlQuerry = $"CALL animals_insert ('{name}', {weight}, {age});";

			// Skapa MySQLCOmmand objekt
			conn.Open();
			MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

			//Exekvera MySQLCommand
			cmd.ExecuteReader();

			//Stänga DB koppling
			conn.Close();
		}
		public static void FetchData(MySqlConnection conn)
		{
			// SQL Querry för INSERT
			string sqlQuerry = "CALL animals_select();";

			// Skapa MySQLCOmmand objekt
			conn.Open();
			MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

			//Exekvera MySQLCommand. Spara resultat i reader
			MySqlDataReader reader = cmd.ExecuteReader();

			//Tömma Animals list
			Animal.animals.Clear();

			//While Loop för att skriva ut resultatet till Konsol
			while (reader.Read())
			{
				//Skriv ut animal till Konsol
				Console.WriteLine($"Name: {reader["animal_name"]}:Weight: {reader["animal_weight"]} Age: {reader["animal_age"]}");

				//Spara data till Lista
				 new Animal(Convert.ToInt32(reader["animals_id"]),(reader["animal_name"].ToString()),Convert.ToInt32(reader["animal_age"]),Convert.ToDouble(reader["animal_weight"]));
			}

			//Stänga DB koppling
			conn.Close();

			Console.WriteLine("Data Fetched successfully! Press any key to continue");
			Console.ReadKey();
		}
		public static void UpdateData(MySqlConnection conn)
		{
			// Om ingen data har hämtas, hämta data
			if ( Animal.animals.Count == 0)
			{
				FetchData(conn);
				Console.Clear();
			}

			int count = 1;
			// Skriva ut lista till Konsol
			foreach (Animal animal in Animal.animals)
			{
				Console.WriteLine($"{count}, {animal.Name},{animal.Weight},{animal.Age}");             
			   count++;
			}
			// Användaren anger numret Count för den animal som vill ta bort.
			Console.WriteLine("Ange det nummer du vill ändra värde på!");
			int input = Convert.ToInt32(Console.ReadLine());

			// Hämta det valda objektet
			Animal selectedAnimal = Animal.animals[input - 1];
			Console.Clear();

			bool update = false;
			// Ge användaren en förfrågan för varje attribut, om det skall uppdateras eller inte.
			Console.WriteLine($"Namn: {selectedAnimal.Name}");
			Console.Write("Skriv in det nya namnet. Lämna blank om oförändrat:");
			string nameInput = Console.ReadLine();

			if (nameInput != "")
			{
				selectedAnimal.Name = nameInput;
				update = true;
			}
		
			// Weight
			Console.WriteLine($"Vikt: {selectedAnimal.Weight}");
			Console.Write("Skriv in den nya vikten.Lämna blank om oförändrat:");
			string weightInput = Console.ReadLine();

			if (weightInput != "")
			{
				selectedAnimal.Weight = Convert.ToDouble(weightInput);
				update = true;
			}
            // Age
            Console.WriteLine($"Ålder: {selectedAnimal.Age}");
            Console.Write("Skriv in den nya åldern.Lämna blank om oförändrat:");
            string ageInput = Console.ReadLine();

            if (ageInput != "")
            {
                selectedAnimal.Age = Convert.ToInt32(ageInput);
                update = true;
            }

            if (update)
			{
				//Skapa SQL Querry med CALL anropp till DB
				string sqlQuerry = $"CALL animals_update" +
					($"{selectedAnimal.Name}, {selectedAnimal.Age}, {selectedAnimal.Id}");

                /*
			   UPDATE animals
			   SET animal_name = animal_name, animal_weight = animal_weight,animal_age = animal_age,;
			   WHERE animal_id = animal_id;

				*/

                // Exekvera SQLQuerry via MySqlConnestion och MySqlCommand
                MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);
				conn.Open();
				cmd.ExecuteReader();
				conn.Close();
			}

			//Anropa FetchData för att skriva ut ny information
			FetchData(conn);
		}
		public static void RemoveData(MySqlConnection conn)
		{
			//Om ingen data har hämtats, hämta data
			if (Animal.animals.Count ==0)
			 
			{
				FetchData(conn);
				Console.Clear();
			}

			int count = 1;
			//Skriva ut lista till Konsol
			foreach (Animal animal in Animal.animals)
			{
				Console.WriteLine($"{count}. {animal.Name}, {animal.Weight} - {animal.Age}");
				count++;
			}

			//Användaren anger nummret Count för den animal som den vill ta bort.
			Console.WriteLine("Ange det nummer du vill ta bort!");
			int input = Convert.ToInt32(Console.ReadLine());

			//Hämta ID värdet av det valda objektet
			int selectedID = Animal.animals[input - 1].Id;

			//Anropa Stored Procuedure med det valda värdet -1's ID värde
			// SQL Querry för INSERT
			string sqlQuerry = $"CALL animals_delete({selectedID});";

			// Skapa MySQLCOmmand objekt
			conn.Open();
			MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

			//Exekvera MySQLCommand.
			cmd.ExecuteReader();

			//Stänger kopplingen
			conn.Close();

			//Anropa och skriv ut den nya tabellen
			Console.Clear();
			FetchData(conn);

		}

        public static void UpdateData2 (MySqlConnection conn)
        {
            //Om ingen data har hämtats, hämta data
            if (Animal.animals.Count == 0)
            {
                FetchData(conn);
                Console.Clear();
            }

            int count = 1;
            //Skriva ut lista till Konsol
            foreach (Animal animal in Animal.animals)
            {
                Console.WriteLine($"{count}. {animal.Name}.{animal.Weight} - {animal.Age}");
                count++;
            }

            //Användaren anger nummret Count för den person som den vill ta bort.
            Console.WriteLine("Ange det nummer du vill ta bort!");
            int input = Convert.ToInt32(Console.ReadLine());

            //Hämta det valda objektet
            Animal selectedAnimal = Animal.animals[input - 1];

            //Ge användaren en förfrågan för varje attribut, om det skall uppdateras eller inte.
            //Vid svar 'ja', låt användaren ange det nya värdet via konsolen.
            bool update = false;
            PropertyInfo[] properties = typeof(Animal).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                //Hoppa över Property ID, skall ej ändras
                if (property.Name == "Id") continue;

                //Age
                //Console.WriteLine($"Ålder: {selectedPerson.Age}");
                Console.WriteLine($"{property.Name}: {property.GetValue(selectedAnimal)}");
                Console.Write($"Skriv in det nya värdet till {property.Name}. Lämna blank om oförändrat:");
                string userInput = Console.ReadLine();

                if (userInput != "")
                {
					//Kontrollera om property är av datatyp string eller int
					if (property.PropertyType == typeof(int)) // typeof(int)
					{
						int value = Convert.ToInt32(userInput);
						property.SetValue(selectedAnimal, value);
					}
					else if (property.PropertyType == typeof(String))
					{
						property.SetValue(selectedAnimal, userInput);
					}
                    update = true;
                }

                //Console.WriteLine(property.GetValue(selectedPerson));
                //property.SetValue(selectedPerson, value);
            }

            if (update)
            {
                //Skapa SQLQuerry med CALL anropp till DB
                string sqlQuerry = $"CALL animals_update('{selectedAnimal.Name}',{selectedAnimal.Weight},{selectedAnimal.Age},{selectedAnimal.Id});";



                /*
             * UPDATE <tabellNamn>
             * SET <kollumnNamn1> = <värde1>, <kollumnNamn2> = <värde2>
             * WHERE <kolumnID> = <obejkt.id>
             * 
             */

                //Exekvera SQLQuerry via MySqlConnection och MySqlCommand
                MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            }

            //Anropa FetchData för att skriva ut ny information
            FetchData(conn);
        }

    }
}
	

		