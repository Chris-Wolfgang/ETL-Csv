type: fix

`CsvLoader` now checks `MaximumItemCount` before pulling the next item from its source, so a one-shot upstream no longer loses one item past the limit, and it honours an already-cancelled token before writing the header row.
