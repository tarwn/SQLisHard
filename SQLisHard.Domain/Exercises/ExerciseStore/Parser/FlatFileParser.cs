using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLisHard.Domain.Exercises.ExerciseStore.Parser
{
	public class FlatFileParser
	{
		private static Dictionary<ParseState, string> States = new Dictionary<ParseState, string>() { 
			{ ParseState.SetId,					"SetId" },
			{ ParseState.SetTitle,				"SetTitle" },
			{ ParseState.SetSummary,			"SetSummary" },
			{ ParseState.FinaleTitle,			"FinaleTitle" },
			{ ParseState.FinaleDetails,			"FinaleDetails" },
			{ ParseState.NewExercise,			"ExerciseId" },
			{ ParseState.ExerciseTitle,			"Title" },
			{ ParseState.ExerciseQuery,			"Query" },
			{ ParseState.ExerciseExplanation,	"Explanation" },
			{ ParseState.ExerciseExample,		"Example" },
			{ ParseState.ExerciseExercise,		"Exercise" },
		};

		private StateTransitions _stateTransitions;

		public FlatFileParser(StateTransitions transitions)
		{
			_stateTransitions = transitions;
		}

		public Queue<Entry> ParseContent(string content)
		{
			var parsedEntries = new Queue<Entry>();
			ParseState prevState = ParseState.SetId;

			var linePattern = "([A-Za-z]+):[ \\t]*((?:[^\\n]|\\n\\t)+)";
			var regex = new Regex(linePattern);
			var matches = regex.Matches(content);

			if (matches.Count == 0)
				throw new ArgumentException("Did not find any matching lines in the provided file");

			for (var i = 0; i < matches.Count; i++)
			{
				var match = matches[i];
				var config = ParseStateFromConfig(match.Groups[1].Value);
				var value = match.Groups[2].Value;

				if (i == 0)
					EnsureStateMatches(_stateTransitions.InitialState, config);
				else
					EnsureTransitionIsValid(prevState, config, value);

				parsedEntries.Enqueue(new Entry(config, value));
				prevState = config;
			}

			EnsureStateMatches(_stateTransitions.FinalState, prevState);

			return parsedEntries;
		}

		private ParseState ParseStateFromConfig(string config)
		{
			var state = States.Where(s => s.Value.Equals(config)).Select(s => s.Key);
			if (!state.Any())
				throw new ArgumentException(String.Format("'{0}' is not a recognized configuration", config));
			else
				return state.Single();
		}

		public void EnsureStateMatches(ParseState expectedState, ParseState actualState)
		{
			if (expectedState != actualState)
				throw new DefinedExerciseFileFormatException(String.Format("Found {0} when expecting {1}", States[actualState], States[expectedState]));
		}

		private void EnsureTransitionIsValid(ParseState prevState, ParseState nextState, string value)
		{
			if (!_stateTransitions.Transitions.ContainsKey(prevState))
				throw new DefinedExerciseFileFormatException(String.Format("{0} is not expected to have any additional values after it", States[prevState]));
			else if (!_stateTransitions.Transitions[prevState].Any(s => s == nextState))
				throw new DefinedExerciseFileFormatException(String.Format("It's not valid to define {1} directly after {0} (value is '')", States[prevState], States[nextState], value));
		}		

		public static StateTransitions DefineTransitions()
		{
			return new StateTransitions();
		}
	}

	public class Entry
	{
		public ParseState Config { get; private set; }
		public string Value { get; private set; }

		public Entry(ParseState config, string value)
		{
			Config = config;
			Value = value;
		}
	}

	public class StateTransitions
	{
		public ParseState InitialState { get; private set; }
		public ParseState FinalState { get; private set; }
		public Dictionary<ParseState, List<ParseState>> Transitions;

		public StateTransitions()
		{
			Transitions = new Dictionary<ParseState, List<ParseState>>();
		}

		public StateTransitions InitialTransition(ParseState parseState)
		{
			InitialState = parseState;
			return this;
		}

		public StateTransitions AddTransition(ParseState currentState, params ParseState[] nextSteps)
		{
			foreach (var nextState in nextSteps)
			{
				if (!Transitions.ContainsKey(currentState))
					Transitions.Add(currentState, new List<ParseState>());

				Transitions[currentState].Add(nextState);
			}
			return this;
		}

		public StateTransitions AddFinalTransition(ParseState parseState)
		{
			FinalState = parseState;
			return this;
		}
	}

	public enum ParseState
	{
		SetId = 0,
		SetTitle = 1,
		SetSummary = 2,
		FinaleTitle = 3,
		FinaleDetails = 4,
		NewExercise = 5,
		ExerciseTitle = 6,
		ExerciseQuery = 7,
		ExerciseExplanation = 8,
		ExerciseExample = 9,
		ExerciseExercise = 10
	}
}
