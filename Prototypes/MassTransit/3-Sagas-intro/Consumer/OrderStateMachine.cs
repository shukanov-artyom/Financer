using Automatonymous;

namespace Consumer
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public int CurrentState { get; set; }
    }

    public interface SubmitOrder
    {
        Guid OrderId { get; }
    }

    public interface OrderAccepted
    {
        Guid OrderId { get; }
    }

    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            // What property represents curent State?
            InstanceState(x => x.CurrentState, Submitted, Complete);

            // How to find an Instance when we receive an Event? we correlate by OrderId
            Event(() => SubmitExpenseReport, x => x.CorrelateById(context => context.Message.OrderId));

            // state transitions declaration (behavior)
            Initially(
                When(SubmitExpenseReport)
                    .TransitionTo(Submitted));

            During(Submitted, 
                When(ExpenseReportProcessed)
                    .TransitionTo(Complete));

            During(Complete, Ignore(SubmitExpenseReport));
        }

        // Available states declaration
        public State Submitted { get; private set; }
        public State Complete { get; private set; }


        // Events declaration which affect state
        public Event<SubmitOrder> SubmitExpenseReport { get; private set; }
        public Event<SubmitOrder> OrderAccepted { get; private set; }
    }
}
