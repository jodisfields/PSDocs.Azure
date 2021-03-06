# Publish to Azure Web App

Markdown generated with PSDocs for Azure can be published as HTML for viewing with Azure App Service.
Using Azure App Service our docs site can be secured for internal use or shared with authorized guests.

!!! Abstract
    This topic covers using a pipeline to publish HTML generated by PSDocs for Azure into an Azure Web App.

## Generating docs site

To publish documentation to an Azure Web App, we need to generate HTML from markdown.
There are several widely used tools that help you do this.
Two open source examples are:

- **MkDocs** &mdash; A Python-based static site generator.
  MkDocs is fast and can be customized with corporate branding fairly easily.
  A wide-range of themes are also available.
- **DocFX** &mdash; A .NET-based static site generator.
  DocFX is slightly easier to get up and running but require more work to customize and brand.

### With MkDocs

Markdown content generated with PSDocs for Azure can be published as HTML with [MkDocs][1].
MkDocs is a command-line tool that converts markdown into HTML.

!!! Tip
    We recommend you start from our [Quick Start template][2].

  [1]: https://www.mkdocs.org/
  [2]: https://aka.ms/ps-docs-azure-quickstart

=== "GitHub Actions"

    Extend your GitHub Actions workflow `.github/workflows/publish-docs.yaml` created previously by completing the following steps:

    - Add the following steps to install and build documentation with MkDocs.

    ```yaml
    - name: Setup Python
      uses: actions/setup-python@v2.2.2
      with:
        python-version: '3.9'
        architecture: 'x64'

    - name: Install dependencies
      run: |
        python3 -m pip install --upgrade pip
        python3 -m pip install wheel
        python3 -m pip install -r requirements-docs.txt

    - name: Build site
      run: mkdocs build
    ```

- [Create a convention][3] to name and add metadata to output markdown files.
  A conventions allows generated markdown to be modified before it is written.
  We use one with MkDocs to add front matter to markdown files and organize them.

- Update action parameters to set output path and reference convention.

```yaml
# Generate markdown files using PSDocs
# Scan for Azure template file recursively in sub-directories
- name: Generate docs
  uses: microsoft/ps-docs@main
  with:
    conventions: AddMkDocsMeta
    modules: PSDocs,PSDocs.Azure
    outputPath: docs/azure/templates/
    prerelease: true
```

- Create a [mkdocs.yaml][4] file in the root of the repository.
  This file configures MkDocs.
  By configuring this file you can change common settings such as theme and layout.

- Create a [requirements-docs.txt][5] file in the root of the repository.
  This file is used by Python to install the required package dependencies.

  [3]: https://github.com/Azure/PSDocs.Azure-quickstart/blob/main/.ps-docs/MkDocs.Doc.ps1
  [4]: https://github.com/Azure/PSDocs.Azure-quickstart/blob/main/mkdocs.yml
  [5]: https://github.com/Azure/PSDocs.Azure-quickstart/blob/main/requirements-docs.txt

<!--

### With DocFX

Markdown content generated with PSDocs for Azure can be published as HTML with [DocFX][2].
DocFX is a command-line tool that converts markdown into HTML.

  [2]: https://dotnet.github.io/docfx/

=== "Azure Pipelines"

    In your Azure DevOps YAML pipeline `.azure-pipelines/build-arm.yaml` created previously:

    - Add a new stage to publish the docs site to Azure App Service.

    ```yaml
    stages:

    # Build pipeline
    - stage: Build
      displayName: Build
      jobs:

      - job:
        displayName: 'Documentation'
        pool:
          vmImage: $(imageName)
        steps:

        # Install pipeline dependencies
        - powershell: ./.azure-pipelines/pipeline-deps.ps1
          displayName: 'Install dependencies'

        - task: UniversalPackages@0
          displayName: 'Download docfx'
          inputs:
            command: download
            vstsFeed: 'Tools'
            vstsFeedPackage: 'docfx'
            vstsPackageVersion: 2.56.2
            downloadDirectory: out/docfx

        - task: UniversalPackages@0
          displayName: 'Download wkhtmltopdf'
          inputs:
            command: download
            vstsFeed: 'Tools'
            vstsFeedPackage: 'wkhtmltopdf'
            vstsPackageVersion: 0.12.5
            downloadDirectory: out/wkhtmltopdf

        # Build documentation
        - pwsh: Invoke-Build BuildDocs
          displayName: 'Build documentation'

        - publish: out/docs/_site
          displayName: 'Publish docs site'
          artifact: docs-site
    ```

## Publishing to Web App

=== "Azure Pipelines"

    In your Azure DevOps YAML pipeline `.azure-pipelines/build-arm.yaml` created previously:

    - Add a new stage to publish the docs site to Azure App Service.

    ```yaml
    # Docs pipeline
    - stage: Publish
      displayName: Publish
      dependsOn: Build
      variables:
        ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/master') }}:
          publishSlot: staging
        ${{ if ne(variables['Build.SourceBranch'], 'refs/heads/master') }}:
          publishSlot: preview
      jobs:
      - job:
        steps:
    #   - template: jobs/docs.deploy.yaml
    #     parameters:
    #       connectionService: '<subscription_name>'
    #       outPath: '$(Build.SourcesDirectory)/out/'
    #       slotName: '$(publishSlot)'

        # Get docs
        - task: DownloadPipelineArtifact@1
          displayName: 'Download docs'
          inputs:
            artifactName: '${{ parameters.artifact }}'
            downloadPath: '${{ parameters.outPath }}docs-site/'

            # Package web site
        - task: ArchiveFiles@2
          displayName: 'Package site'
          inputs:
            archiveType: zip
            includeRootFolder: false
            rootFolderOrFile: '${{ parameters.outPath }}docs-site/'
            archiveFile: '${{ parameters.outPath }}docs-site.zip'

        # Deploy web site
        - task: AzureRmWebAppDeployment@4
          displayName: 'Deploy docs site'
          inputs:
            azureSubscription: '${{ parameters.connectionService }}'
            WebAppName: ${{ parameters.appName }}
            deployToSlotOrASE: true
            ResourceGroupName: ${{ parameters.resourceGroup }}
            SlotName: ${{ parameters.slotName }}
            DeploymentType: zipDeploy
            packageForLinux: '${{ parameters.outPath }}docs-site.zip'

    ```

-->

## Publishing docs

With documentation generated as HTML the content can be published to an Azure web app.

=== "GitHub Actions"

    Extend your GitHub Actions workflow `.github/workflows/publish-docs.yaml` created previously by completing the following steps:

    - Add the following steps to publish documentation to Azure App Service.

    ```yaml
    - name: Azure Login
      uses: azure/login@v1.3.0
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}

    - name: Publish to Azure
      run: |
        cd ./site
        az webapp up -l '<enter>' -n '<enter>' -g '<enter>' --subscription '<enter>' --html
    ```

    - Create an [deployment credentials][6] `AZURE_CREDENTIALS` for the workflow to use to authenticate to Azure.
    - Set `-l` with the location of the Web App.
      e.g. `eastus`.
    - Set `-n` with the name of the Web App.
    - Set `-g` with the name of the Resource Group containing the Web App.
    - Set `--subscription` with the name or GUID of the subscription to deploy to.

  [6]: https://github.com/azure/login#configure-deployment-credentials

## Configuring authorization

Azure Web Apps have built-in support for integrated with Azure AD.
By using this feature Azure AD takes care of all the heavy lifting in regards to auth.

Read [Configure your App Service or Azure Functions app to use Azure AD login][7] to find out how to configure it.

  [7]: https://docs.microsoft.com/azure/app-service/configure-authentication-provider-aad
