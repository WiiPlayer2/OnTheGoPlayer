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

        post {
            always {
                xunit thresholds: [failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')], tools: [NUnit3(deleteOutputFiles: true, failIfNotNew: true, pattern: 'OnTheGoPlayer.Test/bin/Release/TestResult.xml', skipNoTestFiles: false, stopProcessingIfError: true)]
            }
        }
    }
}
