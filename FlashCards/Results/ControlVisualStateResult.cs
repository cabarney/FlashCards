using System;
using Caliburn.Micro;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FlashCards.Results
{
    public class ControlVisualStateResult : VisualStateResult
    {
        private readonly string _controlName;


        public ControlVisualStateResult(string stateName, string controlName, bool useTransitions = true) : base(stateName, useTransitions)
        {
            _controlName = controlName;
        }

        private Control FindChild(Control parent, string name)
        {
            return parent.FindName(name) as Control;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            if (!(context.View is Control))
                throw new InvalidOperationException("View must be a Control to use VisualStateResult");

            var view = (Control)context.View;

            var control = FindChild(view, _controlName);

            VisualStateManager.GoToState(control, StateName, UseTransitions);

            OnCompleted();
        }
    }
}