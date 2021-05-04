using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace test{
	class X
	{ 
		static void Main(string[] args)
		{

			FileStream stream = null;
			StreamWriter writer = null;

			try{
				string fileName = "testlog.txt";

				stream = new FileStream(
					fileName,
					FileMode.Create,
					FileAccess.Write,
					FileShare.None,
					8
					);

				writer = new StreamWriter(stream);

				foreach(string str in args){
					writer.WriteLine(str);
				}
				writer.Flush();
			}
			finally
			{
				if(stream != null) stream.Close();
			}
		}

		private void saveFile(){
		}
	}
}
