# Health Advisor

This agent assists users in understanding their health symptoms and provides recommendations on when to seek medical attention. It leverages AI capabilities to analyze user-reported symptoms and offers guidance based on established medical knowledge.

It demonstrates the following fundamentals:

- Knowledge - uploaded document about protein types
- Topic basics - collecting and using user input
- Summarizing symptoms using AI Builder Prompt
- Post to Teams channel using a flow
- Lookup basic user information from Microsoft Graph (e-mail, phone number) using a flow

## Main Implementation Components

- Flow: Summarize Patient Symptoms using AI Builder Prompt
- Topic: Check Symptoms
- Topic: Post to Teams using a flow
- Topic: Lookup basic user information from Microsoft Graph

## Sample Conversation

User: Sometimes i go to the gym. what different protein types are there. which is best to take shortly before the workout

### Flow: Summarize Patient Symptoms using AI Builder Prompt

User: I have been experiencing headaches and a sore throat for the past three days. What could be the cause? What could I do to feel better?

### Topic: Post to Teams

User: Post this information to our Teams channel to inform colleagues about my symptoms.

### Topic: Resolve E-Mail Address

User: Can you help me find the email address and phone number of my colleague Ernst Stöger??
