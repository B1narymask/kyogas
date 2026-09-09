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
			// this whole thing is useless because of the change in type markers...
			/*if (!File.Exists(file)) {
					WriteLine("external.fileSys: The file you passed does not exist in this context.");
					return;
				}
				
			for (int i = 0; i < File.ReadAllLines(path).Length; i++) {
				string line = File.ReadAllLines(path)[i];
				string[] parts = line.split(':');

				if (key == parts[0]) {
					if (key[0] == '-') parts[1] = $"\"{newVal}\"";
					switch(key[0]) {
						case '#': 
							if (IsIt.Int(newVal, 0)) parts[1] = newVal; break;
							else return;
						case '%': 
							if (IsIt.byte(newVal, 0)) parts[1] = newVal; break;
							else return;
						case '-': parts[1] = newVal;break;
						case ''
					}
				}

			}*/
		}
}