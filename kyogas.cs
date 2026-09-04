// this file is still incomplete - wer

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
						WriteLine("external.fileSys: The file you passed does not exist in this context.");
						return;
				}

				using (StreamWriter sw = new StreamWriter(file)) {
					WriteLine("lol.whoops: I gave up sorry - Wer");
					return;
				}
		}
		public void update(string key, string newVal, string path) {
			if (!File.Exists(file)) {
					WriteLine("external.fileSys: The file you passed does not exist in this context.");
					return;
				}
				
			foreach (var line in File.ReadAllLines(path)) {
				string[] parts = line.split(':');
				if (key == parts[0]) {
					if (key[0] == '-') parts[1] = $"\"{newVal}\"";
					switch(key[0])
				}

			}
		}
}