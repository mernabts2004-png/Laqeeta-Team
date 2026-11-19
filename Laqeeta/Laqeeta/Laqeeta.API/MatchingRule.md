### How Does the Matching System Work?

When a passenger submits a lost-item report, the system searches for found items that may match the report.

The system considers an item a possible match only when all the following conditions are met:

1. **Same Category:** The found item must have the same category as the lost item (e.g., Phone, Wallet, or Bag).

2. **Same Metro Line:** The found item and the lost report must belong to the same metro line.

3. **Same Direction:** Both items must have the same metro direction (e.g., Northbound or Southbound).

4. **Date Range:** The found date must be within three days before or after the reported lost date (±3 days).

5. **Available Item:** The found item's status must be `Found`. Items that are already reserved or returned are excluded.

### Report Status Updates

* `Searching`: No possible matches were found.
* `PossibleMatch`: At least one possible match was found.
* `ReadyToCollect`: A staff member approved the ownership claim.
* `Closed`: The item was handed over to its owner.

### Important Note

The matching system suggests possible matches based on the available information. It does not automatically confirm ownership. Staff members must verify the passenger's identity and, for high-value items, the secret answer before approving the claim.
