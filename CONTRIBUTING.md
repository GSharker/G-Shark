# Contributing to G-Shark
We love your input! Any contributions are appreciated, no matter how big or small.

### Any contributions you make will be under the MIT Software License
In short, when you submit code changes, your submissions are understood to be under the same [MIT License](http://choosealicense.com/licenses/mit/) that covers the project. Feel free to contact the maintainers if that's a concern.

### Making changes.
Unsure where to begin contributing to G-Shark? You can start by looking through the issues:
We use GitHub issues to track:
	* Reporting a bug
 	* Discussing the current state of the code
	* Submitting a fix
	* Proposing new features

if you are submitting a bug or suggesting an enhancement provide more information as possible.
### Bugs: 
* **Use a clear and descriptive title** for the issue to identify the problem.
* **Describe the exact steps which reproduce the problem** in as many details as possible.
	When listing steps, **don't just say what you did, but explain how you did it**.
* **Provide specific examples to demonstrate the steps**. Include links to files or GitHub projects, or copy/pasteable snippets, which you use in those examples.
* **Describe the behavior you observed after following the steps** and point out what exactly is the problem with that behavior.
* **Explain which behavior you expected to see instead and why.**

### Enhancement:
* **Use a clear and descriptive title** for the issue to identify the suggestion.
* **Explain why this enhancement would be useful** to for the library and users.
	
If the issues is approved, you will be assigned to the issue.
Once you get assegned to an issue you can start coding!

### All Code Changes Happen Through Pull Requests.
1. **Fork the repo** and create your branch from `develop`, we use `develop` for continuous development, so it will be always the most updated, `master` is only for releases.
2. **Create a topic branch** using the tags `feature`, `bug`, `refactor` based on the type if issue.
	* This should usally branch of from master.
	* Please avoid working directly on the `master` branch.
	* To quickly create a topic branch based on master, run `git checkout -b refactor/<name-of-the-topic>` example `refactor/nurbs-curve`.
3. From the topic branch **crete your branches**, this helps to keep track of sub-tasks that defines the issues.
	* To quickly create a topic branch based on master, run `git checkout -b dev/<initial name and surname>/<name-of-the-task>` example `dev/mibi/review-point-at`.
4. If you've added code that should be tested, **add tests**.
5. If you've changed APIs, **update the documentation**.
6. **Ensure the test suite passes**.
7. Make sure your code lints.
8. Issue that pull request!
	* In the pull request, **outline what you did and link it to the specific issue** that you are resolving. This is a tremendous help for us in evaluation and acceptance.
	* Once the pull request is in, **please do not delete the branch or close the pull request** (unless something is wrong with it).

### Respond to feedback on pull request

We may have feedback for you to fix or change some things. We generally like to see that pushed against the same topic branch (it will automatically update the Pull Request).
If we have comments or questions when we do evaluate it and receive no response, it will probably lessen the chance of getting accepted. Eventually, this means it will be closed if it is not accepted. Please know this doesn't mean we don't value your contribution, just that things go stale. If in the future you want to pick it back up, feel free to address our concerns/questions/feedback and reopen the issue/open a new PR (referencing old one).
