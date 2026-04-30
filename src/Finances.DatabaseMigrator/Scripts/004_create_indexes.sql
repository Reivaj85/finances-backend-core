create index if not exists ix_recurring_expenses_household_id
    on finance.recurring_expenses (household_id);

create index if not exists ix_recurring_expenses_category_id
    on finance.recurring_expenses (category_id);

create index if not exists ix_recurring_expenses_status
    on finance.recurring_expenses (status);
