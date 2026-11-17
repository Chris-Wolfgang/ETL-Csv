type: fix

`InitialRecordIndex` now discards the preamble as raw lines before the CSV parser sees them. Previously a blank line, a comment line (with `AllowComments`) or a quote character in the banner made the skip overshoot and consume the header or the whole file. Reported line numbers are unchanged (physical source lines).
