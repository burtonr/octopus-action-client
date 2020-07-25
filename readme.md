# OctopusDeploy Client for GitHub Actions
**This is a work in progress**

This project was created as a way to interact with the [Octopus.Client](https://octopus.com/docs/octopus-rest-api/octopus.client) through [GitHub Actions](https://help.github.com/en/actions)

GitHub Actions pass user input from their `workflow.yml` file to the action handler via environment variables. The inputs and expected environment variables for this action handler are listed below.

- OCTOPUS_URL
    - `INPUT_OCTOPUS_URL`
- API_KEY
    - `INPUT_API_KEY`
- PROJECT_NAME
    - `INPUT_PROJECT_NAME`
- RELEASE_VERSION
    - `INPUT_RELEASE_VERSION`
    - This is passed to Octopus Deploy as-is, so it must match their requirements:
        - You can use standard version numbers with two, three or four components: (`2.3`, `2.3.16`, `2.3.16.384`)
        - You can also include [semantic version](https://g.octopushq.com/SemVer) tags: (`2.3.16-beta`, `2.3.16.384-pre-release`)
    - This version must exactly match the version of the package that will be released or deployed
- RELEASE_ID
    - `INPUT_RELEASE_ID`
    - _Used only for the "deploy" action_
- ENVIRONMENT_NAME
    - `INPUT_ENVIRONMENT_NAME`
- SPACE_NAME
    - `INPUT_SPACE_NAME`
    - _Optional: default value: `Default`_
- CHANNEL_NAME
    - `INPUT_CHANNEL_NAME`
    - _Optional: default value: `Default`_

# Run Locally
This action can be run and invoked locally by running the Docker container and passing the inputs as environment variable parameters and the desired action (in the example below, this is `release` to create a new Octopus Release)

```
docker run --rm \
    -e INPUT_OCTOPUS_URL="https://your.octopus.url.com" \
    -e INPUT_API_KEY="YourGeneratedApiKey" \
    -e INPUT_PROJECT_NAME="Your Project" \
    -e INPUT_RELEASE_VERSION="1.0.0" \
    octopus-action-client:dev release
```
