create table if not exists finance.income_sources (
    id uuid primary key,
    household_id uuid not null references core.households(id),
    name varchar(160) not null,
    expected_amount numeric(18, 2) not null,
    currency char(3) not null,
    frequency varchar(32) not null,
    stability varchar(32) not null,
    status varchar(32) not null,
    created_at timestamp with time zone not null default now(),
    constraint ck_income_sources_expected_amount_non_negative check (expected_amount >= 0),
    constraint ck_income_sources_currency_iso check (currency ~ '^[A-Z]{3}$'),
    constraint ck_income_sources_frequency check (frequency in ('monthly')),
    constraint ck_income_sources_stability check (stability in ('stable', 'variable', 'uncertain')),
    constraint ck_income_sources_status check (status in ('active', 'inactive'))
);

create table if not exists finance.income_records (
    id uuid primary key,
    income_source_id uuid not null references finance.income_sources(id),
    amount numeric(18, 2) not null,
    currency char(3) not null,
    received_on date not null,
    created_at timestamp with time zone not null default now(),
    constraint ck_income_records_amount_non_negative check (amount >= 0),
    constraint ck_income_records_currency_iso check (currency ~ '^[A-Z]{3}$')
);
