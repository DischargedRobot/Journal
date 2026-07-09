import Box from "@mui/material/Box"
import Stack from "@mui/material/Stack"
import { Fragment, ReactNode, useState } from "react"

interface Props {
	steps: ReactNode[]
}

const Wizard = (props: Props) => {
	const { steps } = props

	const [currentStep, setCurrentStep] = useState(0)

	return (
		<Box className="flex flex-row gap-4">
			{steps.map((step, index) => (
				<Fragment key={index}>
					<Stack direction="row" spacing={2}>
						<Box
							className="rounded-full"
							sx={{
								width: "10%",
								height: "10%",
								backgroundColor:
									index === currentStep
										? "primary.main"
										: "secondary.main",
							}}
						>
							{index + 1}
						</Box>
						{step}
					</Stack>
				</Fragment>
			))}
		</Box>
	)
}

export default Wizard
