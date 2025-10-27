# Employee Success Buddy

This is a no-code agent designed to assist with HR-related tasks.

Creation:

Create a new agent based on Career Coach template. Update the Description to contain the following:

```plaintext
Career Coach offers personalized career advice, goals and action plans. It also helps with employee onboarding
```

Update Description and starter prompts as follows:

```plaintext
Add the instructions that this agent also helps with onboarding. Add the instructions that this agent also helps with onboarding. I will add the knowledge later on Replace the Networking starter prompt with: Important Apps: Provide a list of important apps and urls for employees in my company. Make sure the agent has the following starter prompts:

Apps: Provide a list of important apps and urls for employees in my company
Vacations: How do I request vacation time?
Hardware Issues: How do I report hardware problems and request replacements?
Resume Tips: How can I improve my resume for job applications?
Hiring Processes & Job: Tell me about hiring processes and internal job applications
Workplace Culture: How can I adapt to and thrive in my workplace culture?
```

### Artifact Creation

- Create an onboarding plan with the following details:

```plaintext
Draft an onboarding plan and checklist for "Employee Success Buddy" agent that helps new hires get started in a large corporation with 500+ users. The plan should include steps for pre-onboarding, first day, first week, and first 90 days, emphasizing access to necessary tools, resources, and team introductions.
```

- Go to [SharePoint Demos](https://integrationsonline.sharepoint.com/sites/copilot-demo/HRDocuments) site and point out the HR Documents library as a knowledge source.

Point out the following Documents:

- Enterprise URL's and Apps.docx
- Guide to Replacing Hardware.docx
- Holiday Regulations.docx
- Remote Working Regulations.docx

#### Vacation Request - Survey

```plaintext
Help me create a vacation request form that includes fields for first name, last name, email address, department, and dates of the requested time off and other common fields in a vacation request form.
```

- Add the form to to the agents instructions for vacation requests:

```plaintext
When asked about vacation requests always provide this link to the vacation request form
```

#### Hardware Problem Reporting - Survey

```plaintext
Help me create a survey to report hardware problems and request replacements for laptops, phones, iPads, and access keys. Please include fields for first name, last name, department, and email address.
```

Add the form the agents instructions for hardware problem reporting: https://forms.office.com/Pages/DesignPageV2.aspx?prevorigin=shell&origin=NeoPortalPage&subpage=design&id=fiQr2eCQaUShKWoyhmwNCpcyhSUYFMRPluwi-LyDpktUMEtSR1RWNE5QRjEyVlhUUUlNNzE3OE5HNC4u

```plaintext
When asked about hardware problem reporting always provide this link to the hardware problem reporting form
```
