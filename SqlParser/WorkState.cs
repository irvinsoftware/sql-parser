using System;
using System.Collections.Generic;
using System.Linq;

namespace Irvin.SqlParser
{
    public class WorkState
    {
        public WorkState(IProgress<WorkState> reporter = null)
        {
            Reporter = reporter;
            OverallStep = new Step(null);
            CurrentStep = OverallStep;
        }

        private IProgress<WorkState> Reporter { get; }
        private Step OverallStep { get; }
        private Step CurrentStep { get; set; }

        public void DefineStep(string description, uint? subStepCount = null)
        {
            Step subStep = CurrentStep.AddSubStep(description);
            CreateSubSteps(subStep, subStepCount);
            Publish();
        }

        public void StartNextStep(uint? subStepCount = null)
        {
            if (OverallStep == CurrentStep)
            {
                CurrentStep = OverallStep.SubSteps.First();
            }
            else
            {
                CurrentStep = CurrentStep.Next();
            }

            CreateSubSteps(CurrentStep, subStepCount);

            CurrentStep.Start();
            Publish();
        }

        private static void CreateSubSteps(Step step, uint? subStepCount)
        {
            if (subStepCount.HasValue)
            {
                for (int i = 0; i < subStepCount; i++)
                {
                    step.AddSubStep();
                }
            }
        }

        public void StartStep(string description, uint? subStepCount = null)
        {
            DefineStep(description, subStepCount);
            CurrentStep = CurrentStep.Next();
            CurrentStep.Start();
            Publish();
        }

        public void StepFinished()
        {
            if (CurrentStep == null)
            {
                throw new InvalidOperationException("Step must be started first.");
            }

            CurrentStep.Finish();
            Publish();
        }

        private void Publish()
        {
            Reporter?.Report(this);
        }

        private class Step
        {
            public Step(Step parent)
            {
                Parent = parent;
                SubSteps = new List<Step>();
                State = StepState.NotStarted;
            }

            private Step Parent { get; }
            public uint SequenceNumber { get; private set; }
            private string Description { get; set; }
            public List<Step> SubSteps { get; }

            public StepState State { get; private set; }

            public uint FinishedStepCount => (uint)
                (SubSteps.Sum(s => CountSubSteps(s, ss => ss.State == StepState.Finished)) +
                 SubSteps.Count(ss => ss.State == StepState.Finished));

            public uint TotalSubSteps => (uint)(SubSteps.Sum(s => CountSubSteps(s)) + SubSteps.Count);

            private static uint CountSubSteps(Step step, Func<Step, bool> predicate = null)
            {
                uint sum = 0;

                foreach (Step subStep in step.SubSteps)
                {
                    sum += CountSubSteps(subStep, predicate);
                    if (predicate != null && predicate(subStep))
                    {
                        sum++;
                    }
                }

                return sum;
            }

            public Ratio? PercentComplete => Ratio.Of(FinishedStepCount, TotalSubSteps);

            public Step AddSubStep(string description = null)
            {
                Step subStep = new Step(this);
                subStep.Description = description;
                SubSteps.Add(subStep);
                subStep.SequenceNumber = (uint)SubSteps.Count;
                return subStep;
            }

            public void Start()
            {
                if (State == StepState.NotStarted)
                {
                    State = StepState.InProgress;
                }

                throw new InvalidOperationException($"Step cannot be started when it is in state '{State}'.");
            }

            public Step Next()
            {
                if (Parent == null)
                {
                    return null;
                }

                Step nextSibling = Parent.SubSteps.FirstOrDefault(x => x.SequenceNumber == this.SequenceNumber + 1);

                if (nextSibling != null)
                {
                    return nextSibling;
                }

                return Parent.Next();
            }

            public void Finish()
            {
                if (State == StepState.InProgress)
                {
                    State = StepState.Finished;
                }

                throw new InvalidOperationException($"Step cannot be completed when it is in state '{State}'.");
            }
        }

        private enum StepState
        {
            NotStarted,
            InProgress,
            Finished
        }
    }
}