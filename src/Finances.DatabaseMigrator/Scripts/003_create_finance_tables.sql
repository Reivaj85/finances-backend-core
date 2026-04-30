create table if not exists finance.recurring_expenses (
    id uuid primary key,
    household_id uuid not null references core.households(id),
    category_id uuid not null references core.categories(id),
    name varchar(160) not null,
    expected_amount numeric(18, 2) not null,
    currency char(3) not null,
    frequency varchar(32) not null,
    status varchar(32) not null,
    created_at timestamp with time zone not null default now(),
    constraint ck_recurring_expenses_expected_amount_non_negative check (expected_amount >= 0),
    constraint ck_recurring_expenses_currency_iso check (currency ~ '^[A-Z]{3}$'),
    constraint ck_recurring_expenses_frequency check (frequency in ('monthly')),
    constraint ck_recurring_expenses_status check (status in ('active', 'inactive', 'cancelled'))
);
