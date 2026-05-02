create index if not exists ix_income_sources_household_id
    on finance.income_sources (household_id);

create index if not exists ix_income_sources_status
    on finance.income_sources (status);

create index if not exists ix_income_records_income_source_id
    on finance.income_records (income_source_id);

create index if not exists ix_income_records_received_on
    on finance.income_records (received_on);
