pipeline {
    agent {
        label 'windows'
    }

    stages {
        stage('Cleanup') {
            steps {
                fileOperations([fileDeleteOperation(includes: '*.zip')])
            }
        }

        stage('Build') {
            steps {
                checkout scm
                powershell './build.ps1 -Target Pack -Configuration Release'
                powershell './build.ps1 -Target Pack -Configuration Debug'
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts '/*.zip'
            }
        }

        stage('Publish') {
            steps {
                ftpPublisher alwaysPublishFromMaster: false, continueOnError: false, failOnError: false, masterNodeName: 'master', paramPublish: [parameterName: ''], publishers: [[configName: 'Webspace', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: 'apps/deploy/OnTheGoPlayer', remoteDirectorySDF: false, removePrefix: 'OnTheGoPlayer/OnTheGoPlayer/bin/Release/app.publish', sourceFiles: 'OnTheGoPlayer/OnTheGoPlayer/bin/Release/app.publish/*']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false ]]
            }
        }
    }

    post {
        always {
            xunit thresholds: [failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')], tools: [NUnit3(deleteOutputFiles: true, failIfNotNew: true, pattern: 'OnTheGoPlayer.Test/bin/Release/TestResult.xml', skipNoTestFiles: false, stopProcessingIfError: true)]
        }
    }
}
