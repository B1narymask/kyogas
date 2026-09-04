using Kiogas;
using System;
using System.IO;
using System.Console;
public class Handler {
		Parser parser = new Parser();
		public Dictionary<string,Data> Get(string f) {
				return parser.parse(f);
		}
		public void save<T>(string file, Dictionary<string,Data> info) {
				if (!File.Exists(file)) {
						WriteLine("external")
				}
		}
}