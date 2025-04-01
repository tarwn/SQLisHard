This is observations as I perform the modernization using Cursor.

1. Cursor seems to be doing well at collecting and performing all the work it needs to do, but it is a lot of content and text flying by on the right

2. This is code I am basically unfamiliar with, since it was mostly written 12+ years ago. Massive inline file edits are hard to read and I find myself accepting file changes with the hope that they are alright, because I don't have the original intent in my head to spot when it has made bad assumptions - this gives me the feeling that a rewrite may have been safer, or a rewrite of all but the core business logic as long as the schema of API and DB and Email around the edges was matched

3. These diffs are unreadable inline like this.

4. Cursor is adding new unit tests to a file it is fixing instead of fixing the ones that are already there

5. Cursor is running the build and tests again before I finished approving the changes it made, which then fail because I haven't approved all the changes yet because I'm still reading

6. When approving change-by-change, the editor does not scroll to the next change to review

7. Editor went completely off the rails, adding new tests, modifying behavior, then failing because I rejected edits and telling me I had to manually make the changes. So trying fresh.