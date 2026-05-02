create or replace view analytics.v_monthly_cash_pressure as
with active_income as (
    select
        household_id,
        currency,
        sum(expected_amount) as expected_income_total
    from finance.income_sources
    where status = 'active'
      and expected_amount > 0
    group by household_id, currency
),
active_expenses as (
    select
        household_id,
        currency,
        sum(expected_amount) as active_recurring_expense_total
    from finance.recurring_expenses
    where status = 'active'
      and expected_amount > 0
    group by household_id, currency
)
select
    coalesce(active_income.household_id, active_expenses.household_id) as household_id,
    date_trunc('month', current_date)::date as period,
    coalesce(active_income.expected_income_total, 0)::numeric(18, 2) as expected_income_total,
    coalesce(active_expenses.active_recurring_expense_total, 0)::numeric(18, 2) as active_recurring_expense_total,
    coalesce(active_expenses.active_recurring_expense_total, 0)::numeric(18, 2) as monthly_cash_pressure,
    (
        coalesce(active_income.expected_income_total, 0)
        - coalesce(active_expenses.active_recurring_expense_total, 0)
    )::numeric(18, 2) as estimated_free_cash_flow,
    coalesce(active_income.expected_income_total, 0) <= 0 as is_incomplete
from active_income
full outer join active_expenses
    on active_expenses.household_id = active_income.household_id
   and active_expenses.currency = active_income.currency;
