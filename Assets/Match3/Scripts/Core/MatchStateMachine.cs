using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Match3.Scripts.Core
{
    public class MatchStateMachine
    {
        private readonly Dictionary<MatchState, IMatchState> _states;
        private IMatchState _currentState;

        public MatchState CurrentState { get; private set; } = MatchState.None;
        public bool IsBusy => _currentState != null;

        public MatchStateMachine(Dictionary<MatchState, IMatchState> states)
        {
            _states = states;
        }

        /// <summary>
        /// Sets the starting state, enters it and executes its logic.
        /// Returns the next state reported by the executed state (for chaining).
        /// Note: CurrentState will be set to <paramref name="startingState"/>.
        /// </summary>
        public async UniTask<MatchState> Initialize(MatchState startingState)
        {
            _currentState?.Exit();
            CurrentState = startingState;
            _currentState = _states[startingState];
            _currentState.Enter();

            return await _currentState.Execute();
        }

        /// <summary>
        /// Forces <see cref="CurrentState"/> to a specific state without executing it.
        /// Useful when a state's Execute() returned a different state and we need to
        /// actually enter that returned state.
        /// </summary>
        public void SetCurrentState(MatchState state)
        {
            CurrentState = state;
            _currentState = _states[state];
        }

        /// <summary>
        /// Transitions to <paramref name="newState"/>, enters it and executes its logic.
        /// Returns the next state reported by the executed state (for chaining).
        /// </summary>
        public async UniTask<MatchState> TransitionTo(MatchState newState)
        {
            if (CurrentState == newState)
                return newState;

            _currentState?.Exit();
            CurrentState = newState;
            _currentState = _states[newState];
            _currentState.Enter();

            return await _currentState.Execute();
        }

        public void ExitCurrent()
        {
            _currentState?.Exit();
            _currentState = null;
            CurrentState = MatchState.None;
        }
    }
}
