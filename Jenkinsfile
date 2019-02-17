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
                xunit([NUnit3(deleteOutputFiles: true, failIfNotNew: true, pattern: 'OnTheGoPlayer.Test/bin/*/TestResult.xml', skipNoTestFiles: false, stopProcessingIfError: true)])
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts '/*.zip'
            }
        }
    }
}
