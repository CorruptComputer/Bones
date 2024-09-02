The AbstractValidators in this project should not touch the database or DbContext, 
they are just there to do basic sanity checks on the input values.


The handlers in here should be fairly small and to the point, add the data to the table or get the value from the table.
You can basically just treat them like stored procedures, no major business logic should be here just make sure the input data is valid.