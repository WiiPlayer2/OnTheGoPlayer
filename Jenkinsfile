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
                powershell './build.ps1 -Target Pack -Configuration Debug'
                powershell './build.ps1 -Target Pack -Configuration Release'
                xunit thresholds: [failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')], tools: [NUnit3(deleteOutputFiles: true, failIfNotNew: true, pattern: 'OnTheGoPlayer.Test/bin/Release/TestResult.xml', skipNoTestFiles: false, stopProcessingIfError: true)]
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts '/*.zip'
            }
        }
    }
}
