using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace SpeedDial {
	public static class ListExtension {
		/// <summary>
		/// Selects a random member contained within the given list.
		/// </summary>
		/// <remarks>
		/// Throws InvalidOperationException if the provided list
		/// is empty.
		/// </remarks>
		public static T Random<T>(this IEnumerable<T> list) {
			if(!list.Any())
				throw new InvalidOperationException("Cannot select a random member of an empty list!");

			return list.ElementAt(Rand.Int(0, list.Count() - 1));
		}

		public static bool Random<T>(this IEnumerable<T> list, out T item) {
			try {
				item = list.Random();
				return true;
			} catch(InvalidOperationException) {
				item = default;
				return false;
			}
		}
	}
}
